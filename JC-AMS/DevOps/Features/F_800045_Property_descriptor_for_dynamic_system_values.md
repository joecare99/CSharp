# Feature: Stellt PropertyDescriptors für dynamische Systemwerte bereit

## Beschreibung

Dieses Feature beschreibt `CValuePropertyDescriptor` als Brücke zwischen Systemwerten und dynamischen Property-APIs.

## Sichtbare technische Bausteine

- `Core\Core\System\Values\CValuePropertyDescriptor.cs`
- `Core\Core\System\Values\CValueCollection.cs`
- `Core\Core\System\Values\CValue.cs`

## Fachlicher Nutzen

- Dynamische Systemwerte erscheinen als Properties in UI- und Designer-Kontexten
- Property-Metadaten wie Name, Beschreibung und Kategorie bleiben verfügbar
- Wertzugriff und Wertsetzung laufen über den Wrapper

## Beobachtete Abläufe

- `Description`, `DisplayName` und `Category` leiten sich aus dem Systemwert ab.
- `GetValue()` und `SetValue()` lesen bzw. schreiben den zugrunde liegenden Wert.
- `CValueCollection` erzeugt PropertyDescriptors für alle enthaltenen Werte.

## Offene Fragen

- Ob Reset-/Serialize-Verhalten künftig vollständiger implementiert werden soll
- Ob der Descriptor-Typ für moderne Property-Systeme angepasst werden sollte
