using System.Collections.Generic;

namespace ScriptedSvgWpf.Dsl;

public sealed record ScriptProgram(IReadOnlyList<Statement> Statements);

public abstract record Statement;

public sealed record BlockStatement(IReadOnlyList<Statement> Statements) : Statement;

public sealed record VariableDeclarationStatement(string TypeName, string Name, Expression Initializer) : Statement;

public sealed record AssignmentStatement(Expression Target, Expression Value) : Statement;

public sealed record ExpressionStatement(Expression Expression) : Statement;

public sealed record IfStatement(Expression Condition, Statement ThenBranch, Statement? ElseBranch) : Statement;

public sealed record WhileStatement(Expression Condition, Statement Body) : Statement;

public sealed record ForStatement(
    Statement? Initializer,
    Expression? Condition,
    Expression? Increment,
    Statement Body) : Statement;

public abstract record Expression;

public sealed record LiteralExpression(object? Value) : Expression;

public sealed record NameExpression(string Name) : Expression;

public sealed record UnaryExpression(TokenKind Operator, Expression Operand) : Expression;

public sealed record BinaryExpression(Expression Left, TokenKind Operator, Expression Right) : Expression;

public sealed record CallExpression(Expression Callee, IReadOnlyList<Expression> Arguments) : Expression;

public sealed record MemberExpression(Expression Target, string Name) : Expression;

public sealed record ArrayExpression(IReadOnlyList<Expression> Items) : Expression;

public sealed record IncrementExpression(Expression Target, int Delta) : Expression;
