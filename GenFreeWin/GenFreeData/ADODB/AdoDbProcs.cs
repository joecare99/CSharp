using System;

namespace GenFree.ADODB
{
    public static class AdoDbProcs
    {
        public static void ADODBStream(string sData, string sFile)
        {
            Type? t = Type.GetTypeFromProgID("ADODB.Stream");
            object? obj2 = Activator.CreateInstance(t);

            if (obj2 != null)
            {
                t.GetProperty("Charset")?.SetValue(obj2, "ANSI");
                t.GetProperty("Mode")?.SetValue(obj2, 3);
                t.GetProperty("Type")?.SetValue(obj2, 2);
                t.GetMethod("Open")?.Invoke(obj2, null);
                if (true)
                {

                    /* instance = obj;
                     array = new object[1] { sData };
                     arguments = array;
                     array2 = new bool[1] { true };
                     */
                    t.GetMethod("WriteText")?.Invoke(obj2, new[] { sData });
                    //obj2.WriteText(sData);
                    /*NewLateBinding.LateCall(instance, null, "WriteText", arguments, null, null, array2, IgnoreReturn: true);
                    if (array2[0])
                    {
                        sData = (string)Conversions.ChangeType(array[0], typeof(string));
                    }*/
                    t.GetMethod("Flush")?.Invoke(obj2, null);
                    //NewLateBinding.LateCall(obj, null, "Flush", new object[0], null, null, null, IgnoreReturn: true);
                    /*
                    instance2 = obj;
                    array3 = new object[1] { FILENAM };
                    arguments2 = array3;
                    array2 = new bool[1]
                    {
                    true
                    }
                    ;
                    */
                    t.GetMethod("SaveToFile")?.Invoke(obj2, new[] { sFile });
                //    obj2.SaveToFile(sFile);
                    /*NewLateBinding.LateCall(instance2, null, "SaveToFile", arguments2, null, null, array2, IgnoreReturn: true);
                    if (array2[0])
                    {
                        FILENAM = (string)Conversions.ChangeType(array3[0], typeof(string));
                    }
                    */
                }
                else
                {
                    // Interaction.MsgBox(Information.Err().Number);
                }
                t.GetMethod("Close")?.Invoke(obj2, null);
                //                            NewLateBinding.LateCall(obj, null, "Close", new object[0], null, null, null, IgnoreReturn: true);

            }

        }
    }
}
