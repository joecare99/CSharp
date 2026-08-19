using System;
using System.Collections.Generic;

namespace ScriptedSvgWpf.Dsl;

public sealed class ScriptParser
{
    private static readonly HashSet<string> TypeNames = new(StringComparer.Ordinal)
    {
        "int", "float", "bool", "string", "Point", "Rect"
    };

    private readonly IReadOnlyList<Token> _tokens;
    private int _current;

    public ScriptParser(IReadOnlyList<Token> tokens)
    {
        _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
    }

    public ScriptProgram Parse()
    {
        var statements = new List<Statement>();
        while (!Check(TokenKind.EndOfFile))
        {
            statements.Add(ParseStatement());
        }

        return new ScriptProgram(statements);
    }

    private Statement ParseStatement()
    {
        if (MatchIdentifier("let"))
        {
            return ParseVariableDeclaration(true);
        }

        if (CheckTypeName())
        {
            return ParseVariableDeclaration(false);
        }

        if (MatchIdentifier("if"))
        {
            return ParseIf();
        }

        if (MatchIdentifier("while"))
        {
            Consume(TokenKind.LeftParen, "Expected '(' after while.");
            var condition = ParseExpression();
            Consume(TokenKind.RightParen, "Expected ')' after while condition.");
            return new WhileStatement(condition, ParseBody());
        }

        if (MatchIdentifier("for"))
        {
            return ParseFor();
        }

        if (Match(TokenKind.LeftBrace))
        {
            return ParseBlockAfterOpeningBrace();
        }

        var expression = ParseExpression();
        if (Match(TokenKind.Equal))
        {
            var value = ParseExpression();
            Consume(TokenKind.Semicolon, "Expected ';' after assignment.");
            return new AssignmentStatement(expression, value);
        }

        if (Match(TokenKind.PlusEqual, TokenKind.MinusEqual, TokenKind.StarEqual, TokenKind.SlashEqual))
        {
            var operation = Previous().Kind switch
            {
                TokenKind.PlusEqual => TokenKind.Plus,
                TokenKind.MinusEqual => TokenKind.Minus,
                TokenKind.StarEqual => TokenKind.Star,
                _ => TokenKind.Slash
            };
            var value = ParseExpression();
            Consume(TokenKind.Semicolon, "Expected ';' after assignment.");
            return new AssignmentStatement(expression, new BinaryExpression(expression, operation, value));
        }

        Consume(TokenKind.Semicolon, "Expected ';' after expression.");
        return new ExpressionStatement(expression);
    }

    private VariableDeclarationStatement ParseVariableDeclaration(bool hasLet)
    {
        var type = Consume(TokenKind.Identifier, "Expected a type name.").Lexeme;
        if (!TypeNames.Contains(type))
        {
            throw Error(Previous(), $"Unknown type '{type}'.");
        }

        var name = Consume(TokenKind.Identifier, "Expected a variable name.").Lexeme;
        Consume(TokenKind.Equal, "Expected '=' in variable declaration.");
        var initializer = ParseExpression();
        Consume(TokenKind.Semicolon, "Expected ';' after variable declaration.");
        return new VariableDeclarationStatement(type, name, initializer);
    }

    private IfStatement ParseIf()
    {
        Consume(TokenKind.LeftParen, "Expected '(' after if.");
        var condition = ParseExpression();
        Consume(TokenKind.RightParen, "Expected ')' after if condition.");
        var thenBranch = ParseBody();
        Statement? elseBranch = null;
        if (MatchIdentifier("else"))
        {
            elseBranch = ParseBody();
        }

        return new IfStatement(condition, thenBranch, elseBranch);
    }

    private ForStatement ParseFor()
    {
        Consume(TokenKind.LeftParen, "Expected '(' after for.");
        Statement? initializer = null;
        if (!Check(TokenKind.Semicolon))
        {
            if (MatchIdentifier("let"))
            {
                initializer = ParseVariableDeclaration(true);
            }
            else if (CheckTypeName())
            {
                initializer = ParseVariableDeclaration(false);
            }
            else
            {
                var initializerExpression = ParseExpression();
                if (Match(TokenKind.Equal))
                {
                    initializer = new AssignmentStatement(initializerExpression, ParseExpression());
                }
                else if (Match(TokenKind.PlusEqual, TokenKind.MinusEqual, TokenKind.StarEqual, TokenKind.SlashEqual))
                {
                    var operation = Previous().Kind switch
                    {
                        TokenKind.PlusEqual => TokenKind.Plus,
                        TokenKind.MinusEqual => TokenKind.Minus,
                        TokenKind.StarEqual => TokenKind.Star,
                        _ => TokenKind.Slash
                    };
                    initializer = new AssignmentStatement(initializerExpression, new BinaryExpression(initializerExpression, operation, ParseExpression()));
                }
                else
                {
                    initializer = new ExpressionStatement(initializerExpression);
                }

                Consume(TokenKind.Semicolon, "Expected ';' after for initializer.");
            }
        }
        else
        {
            Advance();
        }

        Expression? condition = null;
        if (!Check(TokenKind.Semicolon))
        {
            condition = ParseExpression();
        }

        Consume(TokenKind.Semicolon, "Expected ';' after for condition.");
        Expression? increment = null;
        if (!Check(TokenKind.RightParen))
        {
            var incrementTarget = ParseExpression();
            if (Match(TokenKind.Equal))
            {
                increment = new BinaryExpression(
                    incrementTarget,
                    TokenKind.Equal,
                    ParseExpression());
            }
            else if (Match(TokenKind.PlusEqual, TokenKind.MinusEqual, TokenKind.StarEqual, TokenKind.SlashEqual))
            {
                var operation = Previous().Kind switch
                {
                    TokenKind.PlusEqual => TokenKind.Plus,
                    TokenKind.MinusEqual => TokenKind.Minus,
                    TokenKind.StarEqual => TokenKind.Star,
                    _ => TokenKind.Slash
                };
                increment = new BinaryExpression(
                    incrementTarget,
                    TokenKind.Equal,
                    new BinaryExpression(incrementTarget, operation, ParseExpression()));
            }
            else
            {
                increment = incrementTarget;
            }
        }

        Consume(TokenKind.RightParen, "Expected ')' after for clauses.");
        return new ForStatement(initializer, condition, increment, ParseBody());
    }

    private Statement ParseBody()
    {
        if (Match(TokenKind.LeftBrace))
        {
            return ParseBlockAfterOpeningBrace();
        }

        return ParseStatement();
    }

    private BlockStatement ParseBlockAfterOpeningBrace()
    {
        var statements = new List<Statement>();
        while (!Check(TokenKind.RightBrace) && !Check(TokenKind.EndOfFile))
        {
            statements.Add(ParseStatement());
        }

        Consume(TokenKind.RightBrace, "Expected '}' after block.");
        return new BlockStatement(statements);
    }

    private Expression ParseExpression() => ParseOr();

    private Expression ParseOr()
    {
        var expression = ParseAnd();
        while (Match(TokenKind.OrOr))
        {
            expression = new BinaryExpression(expression, Previous().Kind, ParseAnd());
        }

        return expression;
    }

    private Expression ParseAnd()
    {
        var expression = ParseEquality();
        while (Match(TokenKind.AndAnd))
        {
            expression = new BinaryExpression(expression, Previous().Kind, ParseEquality());
        }

        return expression;
    }

    private Expression ParseEquality()
    {
        var expression = ParseComparison();
        while (Match(TokenKind.EqualEqual, TokenKind.BangEqual))
        {
            expression = new BinaryExpression(expression, Previous().Kind, ParseComparison());
        }

        return expression;
    }

    private Expression ParseComparison()
    {
        var expression = ParseTerm();
        while (Match(TokenKind.Less, TokenKind.LessEqual, TokenKind.Greater, TokenKind.GreaterEqual))
        {
            expression = new BinaryExpression(expression, Previous().Kind, ParseTerm());
        }

        return expression;
    }

    private Expression ParseTerm()
    {
        var expression = ParseFactor();
        while (Match(TokenKind.Plus, TokenKind.Minus))
        {
            expression = new BinaryExpression(expression, Previous().Kind, ParseFactor());
        }

        return expression;
    }

    private Expression ParseFactor()
    {
        var expression = ParseUnary();
        while (Match(TokenKind.Star, TokenKind.Slash, TokenKind.Percent))
        {
            expression = new BinaryExpression(expression, Previous().Kind, ParseUnary());
        }

        return expression;
    }

    private Expression ParseUnary()
    {
        if (Match(TokenKind.Bang, TokenKind.Minus, TokenKind.Plus))
        {
            return new UnaryExpression(Previous().Kind, ParseUnary());
        }

        if (Match(TokenKind.PlusPlus, TokenKind.MinusMinus))
        {
            var incrementToken = Previous();
            return new IncrementExpression(ParseUnary(), incrementToken.Kind == TokenKind.PlusPlus ? 1 : -1);
        }

        return ParsePostfix();
    }

    private Expression ParsePostfix()
    {
        var expression = ParsePrimary();
        while (true)
        {
            if (Match(TokenKind.LeftParen))
            {
                var arguments = new List<Expression>();
                if (!Check(TokenKind.RightParen))
                {
                    do
                    {
                        arguments.Add(ParseExpression());
                    }
                    while (Match(TokenKind.Comma));
                }

                Consume(TokenKind.RightParen, "Expected ')' after arguments.");
                expression = new CallExpression(expression, arguments);
            }
            else if (Match(TokenKind.Dot))
            {
                var name = Consume(TokenKind.Identifier, "Expected a member name after '.'.").Lexeme;
                expression = new MemberExpression(expression, name);
            }
            else if (Match(TokenKind.PlusPlus, TokenKind.MinusMinus))
            {
                expression = new IncrementExpression(expression, Previous().Kind == TokenKind.PlusPlus ? 1 : -1);
            }
            else
            {
                break;
            }
        }

        return expression;
    }

    private Expression ParsePrimary()
    {
        if (Match(TokenKind.Integer))
        {
            return new LiteralExpression(int.Parse(Previous().Lexeme, System.Globalization.CultureInfo.InvariantCulture));
        }

        if (Match(TokenKind.Float))
        {
            return new LiteralExpression(double.Parse(Previous().Lexeme, System.Globalization.CultureInfo.InvariantCulture));
        }

        if (Match(TokenKind.String))
        {
            return new LiteralExpression(Previous().Lexeme);
        }

        if (MatchIdentifier("true"))
        {
            return new LiteralExpression(true);
        }

        if (MatchIdentifier("false"))
        {
            return new LiteralExpression(false);
        }

        if (MatchIdentifier("null"))
        {
            return new LiteralExpression(null);
        }

        if (Match(TokenKind.Identifier))
        {
            return new NameExpression(Previous().Lexeme);
        }

        if (Match(TokenKind.LeftParen))
        {
            var expression = ParseExpression();
            Consume(TokenKind.RightParen, "Expected ')' after expression.");
            return expression;
        }

        if (Match(TokenKind.LeftBracket))
        {
            var items = new List<Expression>();
            if (!Check(TokenKind.RightBracket))
            {
                do
                {
                    items.Add(ParseExpression());
                }
                while (Match(TokenKind.Comma));
            }

            Consume(TokenKind.RightBracket, "Expected ']' after array.");
            return new ArrayExpression(items);
        }

        throw Error(Peek(), "Expected an expression.");
    }

    private bool Check(TokenKind kind) => Peek().Kind == kind;

    private bool CheckIdentifier(string name) => Check(TokenKind.Identifier) &&
        string.Equals(Peek().Lexeme, name, StringComparison.OrdinalIgnoreCase);

    private bool CheckTypeName() => Check(TokenKind.Identifier) && TypeNames.Contains(Peek().Lexeme);

    private bool MatchIdentifier(string name)
    {
        if (!CheckIdentifier(name))
        {
            return false;
        }

        Advance();
        return true;
    }

    private bool Match(params TokenKind[] kinds)
    {
        foreach (var kind in kinds)
        {
            if (Check(kind))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private Token Consume(TokenKind kind, string message)
    {
        if (Check(kind))
        {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private Token Advance()
    {
        if (!Check(TokenKind.EndOfFile))
        {
            _current++;
        }

        return Previous();
    }

    private Token Peek() => _tokens[_current];

    private Token Previous() => _tokens[_current - 1];

    private ScriptSyntaxException Error(Token token, string message) => new(message, token.Line, token.Column);
}
