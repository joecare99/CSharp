Borland hat die interne Dateispezifikation von Paradox nie offiziell vollständig offengelegt. Die genaue Struktur wurde jedoch über die Jahre durch Reverse-Engineering detailliert aufgeschlüsselt (u. a. im Zuge von *pxlib*, *Lazarus/FPC* und Fachartikeln wie dem *Delphi Informant*).

Ein **Secondary Index** in Paradox besteht – anders als der primäre Index (`.PX`) – immer aus **einem Dateipaar** (`.X**` und `.Y**`), das als modifizierter **B-Tree** organisiert ist.

---

### 1. Das Dateinamens- und Erweiterungsschema

Je nachdem, wie der Sekundärindex definiert ist, vergibt Paradox/BDE unterschiedliche Dateiendungen:

* **Einzelfeld-Index (Single Field):**
  * `.Xnn` und `.Ynn`
  * `nn` ist die hexadezimale Feldnummer (1-basiert) der indizierten Spalte in der `.DB`-Tabelle (z. B. `.X01`/`.Y01` für Feld 1, `.X0A`/`.Y0A` für Feld 10).
* **Zusammengesetzter Index (Composite/Multi-field) oder Case-Insensitive:**
  * `.XGn` und `.YGn` (wobei `n` ein Zähler von `0` bis `F` ist, z. B. `XG0` / `YG0`).

---

### 2. Die Aufteilung in zwei Dateien (`.X` und `.Y`)

Paradox trennt die Indexhierarchie physisch in zwei Ebenen:

| Datei | Funktion | Beschreibung |
| :--- | :--- | :--- |
| **`.Xnn` / `.XGn`** | **Index-Root & Branch-Nodes** | Enthält den Index-Header und die **oberen B-Tree-Knoten (Verzweigungsblöcke)**. Sie dient dem schnellen Durchwandern des Baumes. |
| **`.Ynn` / `.YGn`** | **Leaf-Nodes (Blattebene)** | Enthält die eigentlichen **Blatt-Datenblöcke**. Hier liegen die sortierten Schlüsselwerte zusammen mit den Zeigern auf die Zeilen. |

---

### 3. Aufbau des Datei-Headers (z. B. in `.Xnn`)

Der Header ist sehr ähnlich zu dem einer Haupttabelle (`.DB`) oder dem Primärindex (`.PX`) aufgebaut (meist 2048 Bytes groß):

* **Offset `0x00 - 0x01` (`RecordSize` / `KeySize`):** Gesamtlänge des indizierten Schlüssels.
* **Offset `0x02 - 0x03` (`HeaderSize`):** Block-/Headergröße.
* **Offset `0x04` (`FileType` / `TableType`):** Typkennung (z. B. Kennzeichnung als Sekundärindex).
* **Offset `0x05` (`TableLevel`):** Paradox-Level (z. B. 4, 5, 7).
* **Offset `0x06 - 0x09` (`RecordCount`):** Anzahl der indizierten Einträge.
* **Offset `0x20 - 0x21` (`BlockSize`):** Größe eines Indexblocks (typisch: 1 KB, 2 KB, 4 KB bis 32 KB).
* **Offset `0x2A` (`IndexFlags`):** Flags (z. B. Bit für `Maintained`, `Case-Insensitive`, `Descending` bei Level 7).
* **Feld-Deskriptoren:** Angaben darüber, welche Felder aus der `.DB`-Datei in welcher Reihenfolge diesen Index bilden.

---

### 4. Aufbau eines Index-Eintrags (Record im B-Tree)

Ein einzelner Eintrag im Indexblock setzt sich wie folgt zusammen:

```text
+------------------------------+---------------------------------------+
|  Index-Schlüssel (Key Data)  |  Datensatz-Referenz (Record Pointer)  |
|  (Variable Feldlänge)        |  (4 oder mehr Bytes)                  |
+------------------------------+---------------------------------------+
```

1. **Index-Schlüssel (Key Data):**
   * Die normalisierten Rohdaten des indizierten Feldes (bzw. der Felder).
   * Bei *Case-Insensitive*-Indizes sind Zeichenketten in Großbuchstaben umgewandelt/normalisiert.
   * Bei Zahlen/Floats wird das Paradox-eigene IEEE-Vorzeichen-Bit-Format genutzt, damit die Byte-Folge binär sortierbar ist (`memcmp`-kompatibel).
2. **Datensatz-Referenz (Record Pointer):**
   * **Bei Tabellen mit Primärschlüssel (Primary Key):** Der Zeiger verweist nicht auf eine feste Dateiposition, sondern enthält den **Primärschlüsselwert (Primary Key)** des Zieldatensatzes! Der Sekundärindex führt also einen Lookup über den Primärindex aus (*Maintained Secondary Index*).
   * **Bei unindizierten Tabellen (Non-Keyed):** Der Zeiger enthält direkt die **Datensatznummer (Record Number)** bzw. `(Block-Nummer, Slot-Offset)` in der `.DB`-Datei.

---

### 5. Wo findet man den Quelltext dazu?

Wenn Sie die genauen C-Strukturen (`struct`) Byte für Byte ansehen möchten, ist der Quellcode von **`pxlib`** die beste Referenz:
* Datei **`pxlib.h`** und **`px_index.c`** im `pxlib`-Sourceforge-Repo.
* Dort sind die C-Strukturen für `px_index_header`, `px_block` und die Algorithmen zum Traversieren der `.X`- und `.Y`-Dateien definiert.