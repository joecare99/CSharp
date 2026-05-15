# Feature: Zeigt und verwaltet Meldungen sowie Systemfeedback

## Beschreibung

Dieses Feature bündelt die Meldungs- und Feedbackverarbeitung für SPS-, Prisma- und Systemmeldungen. Die Solution enthält dafür spezialisierte Message-Klassen und Pools.

## Sichtbare technische Bausteine

- `Message\Message\CMessageServiceSPS.cs`: SPS-nahe Betriebszustands- und Solltaktzeitmeldung
- `Message\Message\CPrisma.cs`: Initialisiert Prisma-Zustände, Systemwerte und Stationen
- `Message\Message\CPrismaStationPool.cs`: Hält und synchronisiert Prisma-Stationen aus der Kommunikationskonfiguration
- `Message\Message\CMessageMonitor.cs`: Zyklische Initialisierung und Pflege von Prisma- und Message-Synchronisation
- `Message\Message\CSyncMessageMessage.cs`: synchronisiert Message-Definitionen und legt fehlende Systemwerte/Einträge an
- `Message\Message\CSyncMessageHistory.cs`: lädt Message- und Systemwerte, hält Historie und triggert Statuspflege
- `Message\Message\CPrismaStation.cs`: kapselt Prisma-Connect, KeepAlive und Report-Aufrufe pro Station
- `Message\Message\CMessageServiceVehiclePool.cs`: verwaltet Prisma-bezogene Fahrzeugmeldungen je AGV
- `Message\Message\CMessageServiceVehicle.cs`: bildet Fahrzeugzustände, Fehler und Betriebszustandswechsel ab
- `Message\Message\CPrismaStoercodePool.cs`: stellt Prisma-Störcodes aus Message-Definitionen bereit
- `Message\Message\CPrismaState.cs`: hält die Prisma-Betriebszustandsdaten
- `Message\Message\CPrismaStoercode.cs`: beschreibt einzelne Prisma-Störcodes aus der Definitionswelt
- `Message\Message\SMessageMessage.cs` und `SMessageMessageDef.cs`: interne Datencontainer für Message- und Definitionsdaten
- `Core\Core\Messaging`: Meldungstypen, Fehlerbehandlung und Protokollierung

## Fachlicher Nutzen

- Betriebszustände können an angebundene Systeme gemeldet werden
- Rückmeldungen aus der Anlage werden zentral gebündelt
- SPS-Feedback kann von allgemeiner Logging-Logik getrennt werden
- Meldungen bleiben fachlich auswertbar

## Beobachtete Abläufe

- Prisma wird nur bei aktivem PRISMA-Service initialisiert.
- Systemwerte werden aus der Konfiguration geladen und in den Betriebszustand übersetzt.
- Stationsdaten werden aus `Communication_CommPartner` synchronisiert.
- Der MessageMonitor stößt zyklisch die Prisma-Aktualisierung und die Message-Synchronisation an.
- `CSyncMessageMessage` gleicht Definitionen in der Datenbank ab und erzeugt fehlende Datensätze.
- `CSyncMessageHistory` führt History-, Systemwert- und Definitionsdaten zusammen und aktualisiert Zustände zyklisch.
- `CPrismaStation` nutzt KeepAlive-Timer und Report-Methoden, um Prisma-Meldungen je Station zu versenden.
- `CMessageServiceVehiclePool` und `CMessageServiceVehicle` koppeln AGV-Zustände an Prisma-Feedback und Störmeldungen.
- `CPrismaStoercodePool` leitet Störcodes aus den Message-Definitionen ab.
- `CPrismaState` bündelt die an Prisma gemeldeten Betriebsdaten wie Bearbeitungsart, Werkstücktyp und Taktzeiten.
- `CPrismaStoercode` normalisiert Störtexte und Codes aus den Message-Definitionen.
- Die internen `SMessage*`-Container strukturieren Abfragen zwischen Datenbank, Sync-Logik und Historienpflege.

## Detaillierte Flüsse

- `CMessageServiceVehicle.SendBetriebszustandswechsel()` ermittelt den aktuellen Betriebszustand und meldet bei Störung den passenden Prisma-Störcode.
- `CMessageServiceVehicle.SetZaehlerstandWechsel()` übergibt Stückzahlen und Typbezeichnungen an die Prisma-Station.
- `CPrismaStation.SendBetriebszustandswechsel()` setzt Bearbeitungsart, Werkstücktyp, Teileidentifikation, Betriebszustand und Störcode in der Prisma-Schnittstelle um.
- `CPrismaStation.ReportBetriebszustand()`, `ReportZaehlerstand()` und `ReportSolltaktzeiten()` senden zyklische oder anforderungsbasierte Rückmeldungen.
- `CSyncMessageHistory.UpdateMessageHistory()` legt Meldungshistorien an, aktualisiert offene Einträge und stößt anschließend Prisma-Statusmeldungen an.

## Relevante Datenmodelle

- `SMessageMessageDef` beschreibt die technische Definition einer Meldung inklusive Prisma-Störcode, Journalisierung und Anzeigeverhalten.
- `SMessageMessage` hält die konkrete Meldungsinstanz samt Verknüpfung zu `System_Value`.
- `SMessageMessageHistory` bildet die zeitliche Historie von Meldungen ab.
- `CPrismaState` bündelt die aktuellen Werte für Betriebszustand, Bearbeitungsart und Taktzeiten.
- `CPrismaStoercode` normalisiert den technischen Störcode und den dazugehörigen Text.
- `CPrismaStation` führt Identität, Verbindung und Report-Zustand einer Prisma-Station zusammen.

## Historienlogik

- Neue Meldungen werden anhand des aktiven Systemwerts als `Come`-Eintrag in die Historie geschrieben.
- Bestehende offene Meldungen werden mit einem `Go`-Zeitpunkt abgeschlossen.
- Die Historie unterscheidet bewusst zwischen Meldungstext, Gruppe, Station und Unterstation.
- Ein einzelner Meldungswechsel kann parallel die Prisma-Kommunikation anstoßen.
- Der Platzbezug wird aus den zugehörigen Systemwerten ermittelt und mitgeführt.

## Datenquellen und Tabellen

- `Communication_CommPartner` liefert die Prisma-Stationen und Unterstationen.
- `System_Value` stellt die aktuellen Werte für Meldung, Zustand und Zähler bereit.
- `Message_MessageDef` definiert die fachliche und technische Meldungszuordnung.
- `Message_Message` hält die aktive Meldungsinstanz.
- `Message_MessageHistory` speichert die zeitliche Historie der Meldungen.
- `Management_vAGVState` liefert die AGV-Zustände für die Prisma-gekoppelten Fahrzeugmeldungen.
- `Message_MessageDef` wird auch für den Störcode-Pool als Grundlage verwendet.

## Offene Fragen

- Welche Meldungen an welche Zielsysteme gesendet werden
- Wie Quittierungen und Zustandswechsel modelliert sind
- Welche Meldungen synchron und welche asynchron behandelt werden

## Fortsetzung

Nächster sinnvoller Leseschritt für dieses Themenfeld sind die verbleibenden Meldungs- und Sync-Klassen im `Message`-Projekt sowie die zugehörigen Tests oder Konfigurationspfade.
