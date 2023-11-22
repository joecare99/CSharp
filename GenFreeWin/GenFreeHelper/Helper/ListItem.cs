using System.Collections;

namespace Helper
{
    public class ListItem
    {
#nullable enable
        //
        // Zusammenfassung:
        //     Speichert den ItemData-Wert für ein Element in einem System.Windows.Forms.ListBox-Steuerelement
        //     oder einem System.Windows.Forms.ComboBox-Steuerelement in einer Anwendung, die
        //     von Visual Basic 6.0 auf die neue Version aktualisiert wurde.
        public object? ItemData;

        //
        // Zusammenfassung:
        //     Speichert den List-Wert für ein Element in einem System.Windows.Forms.ListBox-Steuerelement
        //     oder einem System.Windows.Forms.ComboBox-Steuerelement in einer Anwendung, die
        //     von Visual Basic 6.0 auf die neue Version aktualisiert wurde.
        public string ItemString;

        //
        // Zusammenfassung:
        //     Initialisiert eine Instanz der Microsoft.VisualBasic.Compatibility.VB6.ListBoxItem-Klasse.
        //
        // Parameter:
        //   ItemString:
        //     Eine Instanz von System.String, die das Listenelement enthält.
        //
        //   ItemData:
        //     Ein Integer, der den ItemData-Wert in Visual Basic 6.0 darstellt.
        public ListItem(string ItemString, object? ItemData = null)
        {
            this.ItemData = ItemData;
            this.ItemString = ItemString;
        }

        //
        // Zusammenfassung:
        //     Konvertiert ein Microsoft.VisualBasic.Compatibility.VB6.ListBoxItem-Element in
        //     ein System.String-Element.
        //
        // Rückgabewerte:
        //     Ein System.String mit dem Wert des Microsoft.VisualBasic.Compatibility.VB6.ListBoxItem.
        public override string ToString()
        {
            return ItemString;
        }

    }

    public static class ListItemExtensions
    {
        public static object? ItemData(this IList items, int Idx)
            => Idx < items.Count && items[Idx] is ListItem l ? l.ItemData : null;
        public static T? ItemData<T>(this IList items, int Idx)
            => Idx < items.Count && items[Idx] is ListItem l && l.ItemData is T t ? t : default;
        public static object? ItemData(this object item)
            => item is ListItem l ? l.ItemData : null;
        public static T? ItemData<T>(this object item)
            => item is ListItem l && l.ItemData is T t ? t : default;
        public static string ItemString(this IList items, int Idx)
            => Idx < items.Count && items[Idx] is ListItem l ? l.ItemString : string.Empty;

        public static void SetString(this IList items, int Idx, string s)
        {
            if (Idx < items.Count && items[Idx] is ListItem l) l.ItemString = s;
        }
    }
}