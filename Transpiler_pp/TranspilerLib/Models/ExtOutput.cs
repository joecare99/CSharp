using System;
using System.Collections.Generic;
using TranspilerLib.Interfaces;

namespace TranspilerConsTest.Model;

public class ExtOutput : IOutput
{

    enum E_State
    {
        Ignore,
        POU,
        Union,
        U_Decl,
        V_Decl,
        V_Type,
        Type,
        Done,
        TypeDerived,
        TypeArray,
        V_Done,
        Function,
        pou_Interface,
        pou_Body,
        pou_St,
    }
    const string sPathIgnore = "/project/contentheader";
    const string scNodePou = "pou";
    const string scNodeUnion = "union";
    const string sPathPOU = "/project/types/pous/" + scNodePou;
    const string sPathUnion = "/project/adddata/data/" + scNodeUnion;
    const string scAttrName = "name";
    const string scAttrPouType = "poutype";
    const string scNodeVariable = "variable";
    const string scNodeType = "type";
    const string scNodeDerived = "derived";
    const string scNodeArray = "array";
    const string scNodeInterface = "interface";
    const string scNodeArrDimension = "dimension";
    const string scAttArrLower = "lower";
    const string scAttArrUpper = "upper";
    const string scNodeReturnType = "returntype";
    const string scNodeInputVars = "inputvars";
    const string scNodeOutputVars = "outputvars";
    const string scNodeLocalVars = "localvars";
    const string scNodeBody = "body";
    const string scNodeST = "st";


    public void Output(IReader reader, Action<string> write, Action<string> debug)
    {
        var xElExists = false;
        string sPath = "";
        var eState = E_State.Ignore;
        var dData = new System.Collections.Generic.Dictionary<string, string>();
        while (reader.Read())
        {
            if (reader.IsStartElement())
            {
                var sName = reader.GetLocalName();
                var xIsEmptyElement = reader.IsEmptyElement;
                sPath += "/" + sName;
                if (sPath.ToLower().StartsWith(sPathIgnore))
                {
                    if (xIsEmptyElement)
                    {
                        sPath = sPath.Remove(sPath.Length - sName.Length - 1, sName.Length + 1);
                    }
                    continue;
                }
                // Debug
                debug($"/{sPath}{Environment.NewLine}");
                eState = NodeStateMachine(eState, sPath, sName, dData, xIsEmptyElement, write, debug);
                xElExists = false;
                var count = reader.GetAttributeCount();
                for (var i = 0; i < count; i++)
                {
                    string sAttrName = reader.GetAttributeName(i);
                    object oAttrValue = reader.GetAttributeValue(i);
                    // Debug 
                    debug($"/{sPath}/{sAttrName} : \"{oAttrValue}\"{Environment.NewLine}");
                    eState = AttribStateMachine(eState, sAttrName, oAttrValue, sName, dData, write);
                }
                if (xIsEmptyElement)
                {
                    sPath = sPath.Remove(sPath.Length - sName.Length - 1, sName.Length + 1);
                }
            }
            else if (reader.IsEndElement())
            {
                var sName = reader.GetLocalName();
                eState = EndNodeStateMachine(write, eState, dData, sName);
                sPath = sPath.Remove(sPath.Length - reader.GetLocalName().Length - 1, reader.GetLocalName().Length + 1);

            }
            else if (reader.HasValue)
            {
                if (sPath.ToLower().StartsWith(sPathIgnore))
                    continue;
                if (eState == E_State.pou_St)
                    write($"{reader.getValue()}{Environment.NewLine}");
                else
                    debug($"/{sPath}/@ : \"{Quoted(reader.getValue().ToString())}\"");
            }
        }

        string Quoted(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
        }
    }

    private static E_State EndNodeStateMachine(Action<string> _out, E_State eState, Dictionary<string, string> dData, string sName)
    {
        switch (eState, sName.ToLower())
        {
            case (E_State.Type, scNodeType):
                eState = E_State.V_Decl;
                break;
            case (E_State.Type, scNodeReturnType):
                eState = E_State.pou_Interface;
                break;
            case (E_State.V_Done, scNodeInputVars):
                _out($"END_VAR{Environment.NewLine}");
                eState = E_State.pou_Interface;
                break;
            case (E_State.V_Done, scNodeOutputVars):
                _out($"END_VAR{Environment.NewLine}");
                eState = E_State.pou_Interface;
                break;
            case (E_State.V_Done, scNodeLocalVars):
                _out($"END_VAR{Environment.NewLine}");
                eState = E_State.pou_Interface;
                break;
            case (E_State.pou_St, scNodeST):
                eState = E_State.pou_Body;
                break;
            case (E_State.pou_Body, scNodeBody):
                eState = E_State.pou_Interface;
                break;
            case (E_State.pou_Interface, scNodePou):
                if (dData.ContainsKey(scAttrPouType) && dData[scAttrPouType].ToLower() == "function")
                {
                    _out($"END_FUNCTION{Environment.NewLine}");
                }
                else
                {
                    _out($"END_FUNCTION_BLOCK{Environment.NewLine}");
                }
                eState = E_State.Done;
                break;
            case (E_State.V_Decl, scNodeVariable):
                eState = E_State.V_Done;
                break;
            case (E_State.V_Done or E_State.U_Decl, scNodeUnion):
                eState = E_State.Done;
                _out($"  END_UNION{Environment.NewLine}END_TYPE{Environment.NewLine}");
                break;
        }

        return eState;
    }

    private static E_State AttribStateMachine(E_State eState, string sAttrName, object sAttrValue, string sName, Dictionary<string, string> dData, Action<string> _out)
    {
        switch (eState, sAttrName.ToLower())
        {
            case (E_State.Union, scAttrName):
                eState = E_State.U_Decl;
                _out($"TYPE {sAttrValue} :{Environment.NewLine}  UNION{Environment.NewLine}");
                break;
            case (E_State.V_Decl, scAttrName):
                eState = E_State.V_Type;
                _out($"    {sAttrValue} : ");
                break;
            case (E_State.Type, scAttrName):
                eState = E_State.V_Type;
                _out($"    {sAttrValue} : ");
                break;
            case (E_State.TypeDerived, scAttrName):
                eState = E_State.Type;
                _out($"{sAttrValue}; {Environment.NewLine}");
                break;
            case (E_State.TypeArray, scAttArrLower) when sName.ToLower() == scNodeArrDimension:
                _out($"{sAttrValue} ..");
                break;
            case (E_State.TypeArray, scAttArrUpper) when sName.ToLower() == scNodeArrDimension:
                _out($" {sAttrValue}] OF ");
                eState = E_State.Type;
                break;
            case (E_State.POU, scAttrName) when sName.ToLower() == scNodePou:
                dData[scAttrName] = sAttrValue.ToString();
                break;
            case (E_State.POU, scAttrPouType) when sName.ToLower() == scNodePou
                && dData.ContainsKey(scAttrName):
                dData[scAttrPouType] = sAttrValue.ToString();
                if (sAttrValue.ToString().ToLower() == "function")
                {
                    _out($"FUNCTION {dData[scAttrName]} : ");
                    eState = E_State.Function;
                }
                else
                    _out($"{sAttrValue.ToString().ToUpper()} {dData[scAttrName]};");
                break;
        }

        return eState;
    }

    private static E_State NodeStateMachine(E_State _eState, string sPath, string sName, Dictionary<string, string> dData, bool xIsEmptyElement, Action<string> _out, Action<string> write)
    {
        E_State eState = _eState;
        switch (eState, sPath.ToLower(), sName.ToLower())
        {
            case (_, sPathPOU, _):

                eState = E_State.POU;
                dData.Clear();
                break;
            case (_, sPathUnion, _):
                eState = E_State.Union;
                break;
            case (E_State.V_Done, _, scNodeVariable):
            case (E_State.U_Decl, _, scNodeVariable):
                eState = E_State.V_Decl;
                break;
            case (E_State.V_Type, _, scNodeType):
                eState = E_State.Type;
                break;
            case (E_State.Type, _, scNodeDerived):
                eState = E_State.TypeDerived;
                break;
            case (E_State.Type, _, scNodeArray):
                eState = E_State.TypeArray;
                _out($"ARRAY[ ");
                break;
            case (E_State.Type, _, _) when xIsEmptyElement:
                _out($"{sName};{Environment.NewLine}");
                break;
            case (E_State.POU, _, scNodeInterface):
                _out($"{sName};{Environment.NewLine}");
                // Debug 
                write($"// eState : {eState} {Environment.NewLine}");
                break;
            case (E_State.Function, _, scNodeReturnType):
                eState = E_State.Type;
                break;
            case (E_State.pou_Interface, _, scNodeInputVars):
                eState = E_State.V_Decl;
                _out($"VAR_INPUT{Environment.NewLine}");
                break;
            case (E_State.pou_Interface, _, scNodeOutputVars):
                eState = E_State.V_Decl;
                _out($"VAR_OUTPUT{Environment.NewLine}");
                break;
            case (E_State.pou_Interface, _, scNodeLocalVars):
                eState = E_State.V_Decl;
                _out($"VAR{Environment.NewLine}");
                break;
            case (E_State.pou_Interface, _, scNodeBody):
                eState = E_State.pou_Body;
                _out($"BEGIN{Environment.NewLine}");
                break;
            case (E_State.pou_Body, _, scNodeST):
                eState = E_State.pou_St;
                break;
            default: break;
        }
        if (_eState != eState)
        {// Debug 
            write($"// eState : {eState} {Environment.NewLine}");
        }
        return eState;
    }

}
