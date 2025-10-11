using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TranspilerLib.Data;
using TranspilerLib.Interfaces;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.Models.Interpreter;

/// <summary>
/// Einfache Interpreter-Implementierung für einen Teil des IEC AST.
/// Aktuell unterstützt: Funktionsaufrufe (aus <see cref="systemfunctions"/>) und Zuweisungen innerhalb eines CodeBlock-Graphen.
/// </summary>
public class IECInterpreter(ICodeBlock codeBlock) : InterpreterBase, IInterpreter
{
   /// <summary>
   /// Wurzel-Codeblock (z.B. Funktion / Programm) ab dem interpretiert wird.
   /// </summary>
   private readonly ICodeBlock _codeBlock = codeBlock;

   /// <summary>
   /// Abbildung von IEC-Reserviertwort (Funktionsname) auf die .NET Methoden-Signaturen.
   /// Mehrere Overloads werden gesammelt; beim Aufruf wird anhand der Argumentanzahl ausgewählt.
   /// </summary>
   public static IDictionary<Enum, IEnumerable<MethodInfo>> systemfunctions = 
        new Dictionary<Enum, IEnumerable<MethodInfo>>() {
            {IECResWords.rw_ABS,typeof(Math).GetMethods().Where(m=>m.Name==nameof(Math.Abs)) },
            {IECResWords.rw_ACOS,typeof(Math).GetMethods().Where(m=>m.Name==nameof(Math.Acos)) },
            {IECResWords.rw_ASIN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Asin)) },
            {IECResWords.rw_ATAN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Atan)) },
         //   {IECResWords.rw_ATAN2,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Atan2)) },
            {IECResWords.rw_CONCAT,typeof(string).GetMethods().Where(m=>m.Name==nameof(string.Concat)) },
            {IECResWords.rw_COS,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Cos)) },
            {IECResWords.rw_DIV,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.DivRem)) },
            {IECResWords.rw_EXP,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Exp)) },
            {IECResWords.rw_INT,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Floor)) },
            {IECResWords.rw_LEN,[typeof(IECInterpreter).GetMethod(nameof(GetStringLength),BindingFlags.NonPublic|BindingFlags.Static)] },
            {IECResWords.rw_LN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Log)) },
            {IECResWords.rw_LOG,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Log10)) },
            {IECResWords.rw_MOD,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.IEEERemainder)) },
        //    {IECResWords.rw_POW,typeof(Math).GetMethod(nameof(Math.Pow)) },
        //??    {IECResWords.rw_ROUND,typeof(Math).GetMethod(nameof(Math.Round)) },
            {IECResWords.rw_SIN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Sin)) },
            {IECResWords.rw_SQRT,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Sqrt)) },
            {IECResWords.rw_TO_STRING,typeof(Convert).GetMethods().Where(m=>m.Name==nameof(Convert.ToString)) },
            {IECResWords.rw_TAN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Tan)) },
            {IECResWords.rw_TRUNC,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Truncate)) },
        };

    /// <summary>
    /// Hilfsfunktion für LEN.
    /// </summary>
    private static int GetStringLength(string s) => s.Length;

    /// <summary>
    /// Callback zur Auflösung unbekannter Identifikatoren (z.B. Variablen / Funktionen), falls nicht in <see cref="systemfunctions"/>.
    /// Default: wirft <see cref="NotImplementedException"/>.
    /// </summary>
    public Func<string, object> ResolveUnknown { get; set; }
        = (s => throw new NotImplementedException());

   /// <summary>
   /// Liefert ein Dictionary aller im Wurzelblock definierten Entitäten (Noch nicht implementiert).
   /// </summary>
   public IDictionary<string, ICodeBlock> Entitys => GetEntitys(_codeBlock);

    private IDictionary<string, ICodeBlock> GetEntitys(ICodeBlock codeBlock)
    {
        // TODO: Sammeln von Deklarationen / Labels / etc.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Interpretiert den angegebenen Codeblock sequentiell.
    /// Erwartet einen Ablauf: Deklarationen ... 'BEGIN' ... Statements.
    /// Unterstützt aktuell nur Zuweisungen mit optionalem Funktionsaufruf rechts.
    /// </summary>
    /// <param name="cb">Startblock (üblicherweise Wurzel einer Funktion).</param>
    /// <param name="parameters">Variablen-Map (Name -&gt; Wert), wird in-place aktualisiert.</param>
    /// <returns>Optionales Ergebnis (derzeit immer <c>null</c>).</returns>
    public object? Interpret(ICodeBlock cb, IDictionary<string,object> parameters)
    {
        var code = cb.Next;
        // Vorlaufen bis BEGIN gefunden wurde
        while (code != null && code.Code != "BEGIN")
            code = code.Next;
        if (code == null)
            throw new InvalidOperationException("BEGIN Block nicht gefunden.");

        var ipd = new InterpData(code.Next);
        // Hauptschleife: sequentielles Abarbeiten der Next-Kette
        while (ipd.pc != null)
        {
            switch (ipd.pc.Type)
            {
                case CodeBlockType.Assignment:
                    ExecuteAssignment(ipd.pc, parameters);
                    break;
                default:
                    throw new NotImplementedException($"Interpreter unterstützt den Blocktyp '{ipd.pc.Type}' noch nicht.");
            }
            ipd.pc = ipd.pc.Next;
        }

        return null;
    }

    /// <summary>
    /// Führt eine einfache Zuweisung aus. Rechts kann optional ein Funktionsaufruf (Name(Arg1,Arg2,...)) stehen.
    /// </summary>
    private static void ExecuteAssignment(ICodeBlock block, IDictionary<string, object> parameters)
    {
        var parts = block.Code.Split('=');
        if (parts.Length != 2)
            throw new FormatException($"Ungültige Assignment-Syntax: '{block.Code}'");
        var left = parts[0].Trim();
        var right = parts[1].Trim();

        if (right.Contains('('))
        {
            var func = right.Split('(')[0];
            var args = right.Split('(')[1].Split(')')[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var values = args.Select(a => parameters[a.Trim()]).ToArray();
            // Funktionsauflösung anhand Name und Argumentanzahl
            var method = systemfunctions
                .First(m => string.Equals(m.Key.ToString(), func, StringComparison.OrdinalIgnoreCase))
                .Value.First(m => values.Length == m.GetParameters().Length);
            var result = method.Invoke(null, values);
            parameters[left] = result!;
        }
        else
        {
            parameters[left] = parameters[right];
        }
    }
}
