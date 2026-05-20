using System.Collections.Generic;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Binds typed IEC identifier expressions to declarations from a compilation unit symbol table.
/// The first implementation slice focuses on assignments and expression trees that are already
/// modeled in the current typed AST subset.
/// </summary>
public static class IecIdentifierBinder
{
    /// <summary>
    /// Binds all supported identifier expressions from the supplied compilation unit.
    /// </summary>
    /// <param name="compilationUnit">The compilation unit that provides declarations and statements.</param>
    /// <returns>The binding result.</returns>
    public static IecBindingResult Bind(IecCompilationUnit compilationUnit)
    {
        var bindings = new List<KeyValuePair<IecIdentifierExpression, IecVariableDeclaration>>();
        var unresolvedIdentifiers = new List<IecIdentifierExpression>();

        foreach (var statement in compilationUnit.Statements)
        {
            BindStatement(statement, compilationUnit.Symbols, bindings, unresolvedIdentifiers);
        }

        return new IecBindingResult(bindings, unresolvedIdentifiers);
    }

    private static void BindStatement(
        IecStatement statement,
        IecSymbolTable symbolTable,
        ICollection<KeyValuePair<IecIdentifierExpression, IecVariableDeclaration>> bindings,
        ICollection<IecIdentifierExpression> unresolvedIdentifiers)
    {
        switch (statement)
        {
            case IecAssignmentStatement assignment:
                BindIdentifier(assignment.Target, symbolTable, bindings, unresolvedIdentifiers);
                BindExpression(assignment.Value, symbolTable, bindings, unresolvedIdentifiers);
                break;
            case IecIfStatement ifStatement:
                BindExpression(ifStatement.Condition, symbolTable, bindings, unresolvedIdentifiers);
                foreach (var thenStatement in ifStatement.ThenStatements)
                {
                    BindStatement(thenStatement, symbolTable, bindings, unresolvedIdentifiers);
                }

                foreach (var elseStatement in ifStatement.ElseStatements)
                {
                    BindStatement(elseStatement, symbolTable, bindings, unresolvedIdentifiers);
                }
                break;
            case IecReturnStatement returnStatement:
                BindExpression(returnStatement.Expression, symbolTable, bindings, unresolvedIdentifiers);
                break;
        }
    }

    private static void BindExpression(
        IecExpression expression,
        IecSymbolTable symbolTable,
        ICollection<KeyValuePair<IecIdentifierExpression, IecVariableDeclaration>> bindings,
        ICollection<IecIdentifierExpression> unresolvedIdentifiers)
    {
        switch (expression)
        {
            case IecIdentifierExpression identifier:
                BindIdentifier(identifier, symbolTable, bindings, unresolvedIdentifiers);
                break;
            case IecFunctionCallExpression functionCall:
                foreach (var argument in functionCall.Arguments)
                {
                    BindExpression(argument, symbolTable, bindings, unresolvedIdentifiers);
                }
                break;
            case IecUnaryExpression unary:
                BindExpression(unary.Operand, symbolTable, bindings, unresolvedIdentifiers);
                break;
            case IecBinaryExpression binary:
                BindExpression(binary.Left, symbolTable, bindings, unresolvedIdentifiers);
                BindExpression(binary.Right, symbolTable, bindings, unresolvedIdentifiers);
                break;
        }
    }

    private static void BindIdentifier(
        IecIdentifierExpression identifier,
        IecSymbolTable symbolTable,
        ICollection<KeyValuePair<IecIdentifierExpression, IecVariableDeclaration>> bindings,
        ICollection<IecIdentifierExpression> unresolvedIdentifiers)
    {
        if (symbolTable.TryGet(identifier.Identifier, out var declaration) && declaration != null)
        {
            bindings.Add(new KeyValuePair<IecIdentifierExpression, IecVariableDeclaration>(identifier, declaration));
        }
        else
        {
            unresolvedIdentifiers.Add(identifier);
        }
    }
}
