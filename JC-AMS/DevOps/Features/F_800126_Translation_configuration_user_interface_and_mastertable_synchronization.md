# Feature: Übersetzungs-Konfigurationsoberfläche und Mastertable-Synchronisation

## Beschreibung

Dieses Feature dokumentiert `WinGUI\CtrlConfigTranslation`, also die Oberfläche zum Suchen, Anzeigen, Bearbeiten, Importieren, Exportieren und Synchronisieren von Übersetzungen.

## Sichtbare technische Bausteine

- `WinGUI\WinGUI\CtrlConfigTranslation.cs`
- `Core\Core\Globalisation\CTranslation.cs`
- `Core\Core\Globalisation\TTranslations.cs`
- `Core\Core\Globalisation\ETranslTable.cs`
- `Core\Core\Globalisation\EAvailableLanguage.cs`
- `Core\Core\SQL\CSQLQuery`

## Fachlicher Nutzen

- Übersetzungseinträge können pro Sprache gesucht und bearbeitet werden
- Lokale und Master-Übersetzungen können verglichen und synchronisiert werden
- Fehlende oder abweichende Texte werden visuell aufgelistet
- Übersetzungsdaten können importiert und exportiert werden
- Nachrichten- und Fehlertext-Tabellen lassen sich aus der Übersetzungslage ableiten

## Beobachtete Abläufe

- Die Oberfläche lädt Sprachen aus `EAvailableLanguage`.
- Die Listensuche filtert `TTranslations.Trl` nach Name, exaktem Treffer und Übersetzungsstatus.
- `ShowDifferences(...)` baut eine Vergleichstabelle zwischen lokaler und Master-Übersetzung auf.
- `btnSynchronize_Click` schreibt lokale und Master-Werte anhand der gewählten Aktion zurück.
- `btnImportFromFile_Click` und `btnExport_Click` koppeln Datei-Import und Dateiexport an die Übersetzungslogik.
- `btnSyncMessageTable_Click` überträgt Fehlertexte in die Message-Tabellen.

## Fachliche Einordnung

Die Komponente ist ein administratives Pflegewerkzeug und damit ein zentraler Bestandteil der Übersetzungswartung. Sie verbindet UI, Konfiguration, Datenbankzugriffe und die globale Übersetzungsregistry.
