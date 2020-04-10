using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.ClassesAndObjects
{
    public class Members
    {
        // Konstanten
        public static readonly String ConstString = "This is a constant string!";

        // Felder
        public static int FieldCount;

        // Delegate
        public static EventHandler OnChange { get; set; }

        // Methoden
        public static void aMethod()
        {
            FieldCount = 3;
        }

        // Eigenschaften
        public static int aProperty
        {
            get { return FieldCount; }
            set
            {
                if (FieldCount != value)
                {
                    OnChange?.Invoke(null, null); 
                    FieldCount = value;
                }
            }
        }

        // Indexer
        public int this[int i]
        {
            get => i + FieldCount; set => FieldCount = value - i;
        }

    }
}
