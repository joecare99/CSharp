using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Data;
using TranspilerLib.Interfaces;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.Models.Interpreter;

public class IECInterpreter(ICodeBlock codeBlock) : InterpreterBase, IInterpreter
{
   ICodeBlock _codeBlock = codeBlock;

   public static IDictionary<Enum, IEnumerable<MethodInfo>> systemfunctions = 
        new Dictionary<Enum, IEnumerable<MethodInfo>>() {
            {IECResWords.rw_ABS,typeof(Math).GetMethods().Where(m=>m.Name==nameof(Math.Abs)) },
            {IECResWords.rw_ACOS,typeof(Math).GetMethods().Where(m=>m.Name==nameof(Math.Acos)) },
            {IECResWords.rw_ASIN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Asin)) },
            {IECResWords.rw_ATAN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Atan)) },
            {IECResWords.rw_ATAN2,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Atan2)) },
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

    private static int GetStringLength(string s) => s.Length;

    public Func<string, object> ResolveUnknown { get; set; }
        = (s => throw new NotImplementedException());

   public IDictionary<string, ICodeBlock> Entitys => GetEntitys(_codeBlock);

    private IDictionary<string, ICodeBlock> GetEntitys(ICodeBlock codeBlock)
    {
        throw new NotImplementedException();
    }

    public object Interpret(ICodeBlock cb, IDictionary<string,object> parameters)
    {
        var decls = cb;
        var code = cb.Next;
        while (code.Code != "BEGIN")
            code = code.Next;
        var ipd = new InterpData(code.Next);
        while (ipd.pc != null)
        {
            switch (ipd.pc.Type)
            {
                case CodeBlockType.Assignment:
                    var left = ipd.pc.Code.Split('=')[0].Trim();
                    var right = Eval
                    if (right.Contains("("))
                    {
                        var func = right.Split('(')[0];
                        var args = right.Split('(')[1].Split(')')[0].Split(',');
                        var values = args.Select(a => parameters[a.Trim()]).ToArray();
                        var method = systemfunctions.First(m => m.Key.ToString() == func).Value.First(m => values.Count() == m.GetParameters().Count());
                        var result = method.Invoke(null, values);
                        parameters[left] = result;
                    }
                    else
                    {
                        parameters[left] = parameters[right];
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            ipd.pc = ipd.pc.Next;
        }

        return null;
    }
}
