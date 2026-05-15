# Feature: Stellt eine einfache Cipher-Utility bereit

## Beschreibung

Dieses Feature beschreibt die einfache Verschlüsselungs-/Entschlüsselungslogik für kurze und längere Texte. Die Funktion wird durch Tests mit bekannten Referenzwerten abgesichert.

## Sichtbare technische Bausteine

- `Core\Core\SSimpleCipher.cs`
- `Core.Tests\Core\SSimpleCipherTests.cs`

## Fachlicher Nutzen

- Kurze Texte und längere Inhalte können verschlüsselt bzw. entschlüsselt werden
- Einfache Geheimhaltung innerhalb der Anwendung wird unterstützt
- Reproduzierbare Testwerte sichern die Implementierung gegen Abweichungen

## Beobachtete Abläufe

- `SimpleEncrypt()` erzeugt einen Base64-ähnlichen Ciphertext für Eingabetexte.
- `SimpleDecrypt()` liefert den Klartext zurück.
- Leere und `null`-Eingaben werden bewusst separat behandelt.
- Die Tests verwenden feste Referenzstrings für Regressionen.

## Offene Fragen

- Ob die Utility noch für aktuelle Sicherheitsanforderungen genügt
- Wo die Cipher-Funktion produktiv eingesetzt wird
- Ob eine stärkere oder standardisierte Verschlüsselung nötig ist
