using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Resources;

namespace MVVM_31a_CTValidation3.Validator
{

    //
    // Zusammenfassung:
    //     Gibt an, dass ein Datenfeldwert erforderlich ist.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NotASecData : ValidationAttribute
    {
        private string _sData;

        //
        // Zusammenfassung:
        //     Ruft einen Wert ab, der angibt, ob eine leere Zeichenfolge zulässig ist, oder
        //     legt diesen Wert fest.
        //
        // Rückgabewerte:
        //     true, wenn eine leere Zeichenfolge zulässig ist, andernfalls false. Der Standardwert
        //     ist falsesein.
        public bool AllowEmptyStrings
        {
            get;
            set;
        }

        //
        // Zusammenfassung:
        //     Initialisiert eine neue Instanz der System.ComponentModel.DataAnnotations.RequiredAttribute-Klasse.
        public NotASecData(string sData)
            : base()
        {
            _sData = sData;
        }

        //
        // Zusammenfassung:
        //     Überprüft, dass der Wert des erforderlichen Datenfelds nicht leer ist.
        //
        // Parameter:
        //   value:
        //     Der zu überprüfende Datenfeldwert.
        //
        // Rückgabewerte:
        //     true wenn die Validierung erfolgreich ist; andernfalls false.
        //
        // Ausnahmen:
        //   T:System.ComponentModel.DataAnnotations.ValidationException:
        //     Der Datenfeldwert lautete null.
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is string text && text == _sData)
            {
                return false;
            }

            return true;
        }
    }
}
