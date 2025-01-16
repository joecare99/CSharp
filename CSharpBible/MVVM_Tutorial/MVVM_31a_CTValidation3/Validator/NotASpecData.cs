using System;
using System.ComponentModel.DataAnnotations;

namespace MVVM_31a_CTValidation3.Validator;


//
// Zusammenfassung:
//     Gibt an, dass ein Datenfeldwert erforderlich ist.
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class NotTheSpecData : ValidationAttribute
{
    private string _sData;

    //
    // Zusammenfassung:
    //     Initialisiert eine neue Instanz der System.ComponentModel.DataAnnotations.RequiredAttribute-Klasse.
    public NotTheSpecData(string sData)
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
    public override bool IsValid(object? value)
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
