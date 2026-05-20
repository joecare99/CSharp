# Feature: Bietet eine einfache TripleDES-basierte Verschlüsselung

## Beschreibung

Dieses Feature beschreibt `SSimpleCipher` als kleine Hilfsklasse für symmetrische Verschlüsselung und Entschlüsselung auf Basis von TripleDES und Base64.

## Sichtbare technische Bausteine

- `Core\Core\SSimpleCipher.cs`
- `Core.Tests\Core\SSimpleCipherTests.cs`
- `System.Security.Cryptography`

## Fachlicher Nutzen

- Texte können einfach verschlüsselt und entschlüsselt werden
- Die Base64-Darstellung erleichtert Transport und Speicherung
- Ein konstanter Schlüssel- und IV-Satz ermöglicht deterministische Testwerte

## Beobachtete Abläufe

- `SimpleEncrypt(...)` kodiert UTF-8-Text per TripleDES und gibt Base64 zurück.
- `SimpleDecrypt(...)` rekonstruiert den Klartext aus Base64 und TripleDES.
- Fehlerfälle werden geloggt und liefern leere Rückgabewerte.
- Die Tests prüfen bekannte Roundtrip- und Referenzwerte.

## Offene Fragen

- Ob die harte Kodierung von Schlüssel und IV aus Sicherheitsgründen ersetzt werden sollte
- Ob die Klasse nur noch für Legacy-Kompatibilität dokumentiert werden muss
