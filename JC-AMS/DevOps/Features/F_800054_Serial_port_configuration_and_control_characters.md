# Feature: Konfiguriert serielle Ports und stellt Steuerzeichen bereit

## Beschreibung

Dieses Feature beschreibt `CSerialPort` als serielle Kommunikationsklasse mit Konfigurationsladung, Standardparametern und vordefinierten Steuerzeichen.

## Sichtbare technische Bausteine

- `Core\Core\Communication\CSerialPort.cs`
- `Core\Core\Components\IConfigurable`
- `Core\Core\DataOperations\SVariableHandling`

## Fachlicher Nutzen

- Serielle Schnittstellen können projektweit einheitlich konfiguriert werden
- Standardwerte erlauben einen robusten Start ohne bestehende Konfiguration
- Steuerzeichen sind zentral als Konstanten verfügbar

## Beobachtete Abläufe

- Der Konstruktor lädt die Konfiguration für ein Gerät/Portprofil.
- `LoadConfiguration(...)` liest Settings und setzt `PortName`, Baudrate, Parity, StopBits und Timeout-Werte.
- Die Klasse definiert Steuerzeichen wie `SOH`, `STX`, `ETX`, `ACK`, `NAK` und weitere ASCII-Kontrollzeichen.
- Der Destruktor schließt den Port über `ClosePort()`.

## Offene Fragen

- Ob die Konfiguration künftig stärker typisiert statt semikolonbasiert geladen werden sollte
- Ob weitere Serienschnittstellen-Optionen dokumentiert oder ergänzt werden müssen
