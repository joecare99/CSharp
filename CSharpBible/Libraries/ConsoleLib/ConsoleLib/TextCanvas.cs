// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="TextCanvas.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary>
//  TextCanvas kapselt zeichnungsorientierte Operationen für eine rechteckige
//  Teilfläche (Viewport) innerhalb einer (möglicherweise globalen) Konsole.
//  Sie stellt Methoden zum Ausgeben von Text, Einzelzeichen, gefüllten
//  Rechtecken sowie Rahmen zur Verfügung. Alle Koordinaten sind relativ
//  zum Ursprung (0,0) des Canvas und werden intern auf die globale
//  Konsolen-Position unter Berücksichtigung der Dimension umgerechnet.
//  Die Implementierung ist (einfach) threadsicher, indem öffentliche
//  Schreiboperationen per lock(this) serialisiert werden.
// </summary>
// ***********************************************************************
using BaseLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib
{
    /// <summary>
    /// Stellt eine Zeichenfläche (logisches Konsolen-Viewport) bereit, auf die mit
    /// relativen Koordinaten gezeichnet werden kann. Die Klasse übernimmt:
    ///  - Verwaltung einer Dimension (Position + Größe als Rectangle)
    ///  - Ausgabe von Text und Zeichen an beliebigen relativen Koordinaten
    ///  - Zeichnen und Füllen von Rechtecken (ASCII-/Pseudo-Konsolen-Grafik)
    ///  - Verwaltung / Setzen von Vorder- und Hintergrundfarben für Ausgaben
    ///  - Einfache Thread-Synchronisation für gleichzeitige Zugriffe
    ///
    /// Hinweise:
    ///  - Es findet keine Validierung statt, ob die Ausgabe über den sichtbaren
    ///    Bereich der physischen Konsole hinaus ragt.
    ///  - Die Methode DrawRect erwartet ein char[] für den Rahmen (siehe Parameterbeschreibung).
    ///  - Hintergrund- und Vordergrundfarben werden unmittelbar am IConsole-Objekt gesetzt.
    /// </summary>
    public class TextCanvas
    {
        /// <summary>
        /// Rechteck, das die linke obere Ecke (Offset) und die Größe (Breite/Höhe)
        /// des Canvas in Bezug auf die globale Konsole beschreibt.
        /// X/Y = Offset relativ zur physischen Konsole.
        /// Width/Height = logische Größe des Zeichenbereiches.
        /// </summary>
        private Rectangle _dimension;

        /// <summary>
        /// Abstraktion der Konsole. Ermöglicht Testbarkeit und Austauschbarkeit
        /// (z. B. Mock für Unit-Tests).
        /// </summary>
        private readonly IConsole _console;

        /// <summary>
        /// Initialisiert eine neue Instanz von TextCanvas mit angegebener Konsole
        /// und Dimension (Viewport).
        /// </summary>
        /// <param name="console">Implementierung von IConsole (z. B. echte Konsole oder Mock).</param>
        /// <param name="dimension">
        /// Ausgangsdimension (Position + Größe) des Canvas im globalen Konsolen-Koordinatensystem.
        /// </param>
        public TextCanvas(IConsole console, Rectangle dimension)
        {
            _dimension = dimension;
            _console = console;
        }

        /// <summary>
        /// Aktuell verwendete Hintergrundfarbe (nur informativ; reale Ausgabe
        /// setzt direkt _console.BackgroundColor).
        /// </summary>
        public ConsoleColor BackgroundColor { get; internal set; }

        /// <summary>
        /// Liefert das aktuelle Begrenzungsrechteck (Clip-Rechteck) des Canvas.
        /// (Der Name "ClipRect" deutet auf Clipping hin, tatsächlich erfolgt
        /// hier nur die Rückgabe der Dimension ohne aktives Clipping.)
        /// </summary>
        public Rectangle ClipRect => _dimension;

        /// <summary>
        /// Aktuell verwendete Vordergrundfarbe (nur informativ; reale Ausgabe
        /// setzt direkt _console.ForegroundColor).
        /// </summary>
        public ConsoleColor ForegroundColor { get; internal set; }

        /// <summary>
        /// Liefert die interne Dimension (inkl. Offset). Änderungen nur über SetDimension().
        /// </summary>
        public Rectangle Dimension => _dimension;

        /// <summary>
        /// Füllt ein Rechteck relativ zum Canvas mit einem wiederholten Zeichen.
        /// Farbeinstellungen (frcolor/bkcolor) werden vor der Ausgabe auf der Konsole gesetzt.
        /// Es wird nur gezeichnet, falls die linke obere Ecke innerhalb des Canvas liegt.
        /// Hinweis: Es findet keine Bounds-Prüfung für die rechte/untere Grenze statt.
        /// </summary>
        /// <param name="dimension">Relatives Rechteck (x,y,width,height) innerhalb des Canvas.</param>
        /// <param name="frcolor">Vordergrundfarbe für den Text/Zeicheninhalt.</param>
        /// <param name="bkcolor">Hintergrundfarbe für die Fläche.</param>
        /// <param name="c">Zeichen, mit dem der Bereich gefüllt wird.</param>
        public void FillRect(Rectangle dimension, ConsoleColor frcolor, ConsoleColor bkcolor, char c)
        {
            lock (this)
            {
                _console.BackgroundColor = bkcolor;
                _console.ForegroundColor = frcolor;

                if (_dimension.Contains(dimension.Location))
                {
                    // Stringzeile einmal aufbauen (Breite des Rechtecks).
                    string sLine = "";
                    for (int j = dimension.X; j < dimension.Right; j++)
                        sLine += c;

                    // Jede Zeile im Rechteck ausgeben.
                    for (int i = dimension.Y; i < dimension.Bottom; i++)
                        _OutTextXY(dimension.X, i, sLine);
                }
            }
        }

        /// <summary>
        /// Zeichnet einen Rahmen (ohne Innenfüllung) um das angegebene Rechteck.
        /// Unterstützt Spezialfälle:
        ///  - Breite == 0 oder Höhe == 0: Keine Ausgabe.
        ///  - Breite == 1: Vertikale Linie
        ///  - Höhe == 1: Horizontale Linie
        ///
        /// Das char[] boarder (sic) wird wie folgt interpretiert (Index):
        ///  0 = horizontale Linie
        ///  1 = vertikale Linie
        ///  2 = linke obere Ecke
        ///  3 = rechte obere Ecke
        ///  4 = linke untere Ecke
        ///  5 = rechte untere Ecke
        ///
        /// Achtung: Param-Name ist (historisch) falsch geschrieben (boarder statt border).
        /// </summary>
        /// <param name="dimension">Relatives Rechteck, dessen Rand gezeichnet wird.</param>
        /// <param name="frcolor">Vordergrundfarbe des Rahmens.</param>
        /// <param name="bkcolor">Hintergrundfarbe (für den Bereich hinter den Rahmenzeichen).</param>
        /// <param name="boarder">Zeichensatzdefinition der Rahmenbestandteile (siehe Beschreibung).</param>
        public void DrawRect(Rectangle dimension, ConsoleColor frcolor, ConsoleColor bkcolor, char[] boarder)
        {
            lock (this)
            {
                if (dimension.Width == 0 || dimension.Height == 0) return;

                // Spezialfall: nur ein Spaltenbreiter vertikaler Strich
                if (dimension.Width == 1)
                {
                    for (int i = dimension.Y; i < dimension.Bottom; i++)
                        _OutTextXY(dimension.Left, i, boarder[1]);
                    return;
                }

                // Spezialfall: nur eine Zeile (horizontale Linie)
                if (dimension.Height == 1)
                {
                    for (int j = dimension.X; j < dimension.Right; j++)
                        _OutTextXY(j, dimension.Top, boarder[0]);
                    return;
                }

                _console.BackgroundColor = bkcolor;
                _console.ForegroundColor = frcolor;

                // Vertikale Linien (nur wenn Startpunkt innerhalb des Canvas liegt).
                if (_dimension.Contains(dimension.Location))
                {
                    for (int i = dimension.Y + 1; i < dimension.Bottom - 1; i++)
                    {
                        _OutTextXY(dimension.Left, i, boarder[1]);
                        _OutTextXY(dimension.Right - 1, i, boarder[1]);
                    }
                }

                // Horizontale Linien zwischen den Ecken aufbauen.
                string sLine = "";
                for (int j = dimension.X; j < dimension.Right - 2; j++)
                    sLine += boarder[0];

                _OutTextXY(dimension.X + 1, dimension.Top, sLine);
                _OutTextXY(dimension.X + 1, dimension.Bottom - 1, sLine);

                // Ecken setzen
                _OutTextXY(dimension.Location, boarder[2]);
                _OutTextXY(dimension.Right - 1, dimension.Top, boarder[3]);
                _OutTextXY(dimension.Left, dimension.Bottom - 1, boarder[4]);
                _OutTextXY(dimension.Right - 1, dimension.Bottom - 1, boarder[5]);
            }
        }

        /// <summary>
        /// Gibt einen String an einer relativen Position (Point) aus.
        /// Thread-sicher durch lock.
        /// </summary>
        /// <param name="place">Relative Zielposition (X,Y) im Canvas.</param>
        /// <param name="s">Auszugebender Text (unverändert).</param>
        public void OutTextXY(Point place, string s)
        {
            lock (this)
                _OutTextXY(place.X, place.Y, s);
        }

        /// <summary>
        /// Gibt einen String an einer relativen Position (Point) aus.
        /// Thread-sicher durch lock.
        /// </summary>
        /// <param name="place">Relative Zielposition (X,Y) im Canvas.</param>
        /// <param name="s">Auszugebender Text (unverändert).</param>
        public void OutTextXY(Point place, string s, ConsoleColor f, ConsoleColor b)
        {
            lock (this)
                _OutTextXY(place.X, place.Y, s,f,b);
        }

        /// <summary>
        /// Gibt ein Zeichen an einer relativen Position (Point) aus.
        /// </summary>
        /// <param name="place">Relative Zielposition (X,Y) im Canvas.</param>
        /// <param name="c">Auszugebendes Zeichen.</param>
        public void OutTextXY(Point place, char c)
        {
            lock (this)
                _OutTextXY(place.X, place.Y, c);
        }

        /// <summary>
        /// Interne Kurzform für Ausgabe eines Zeichens per Point.
        /// Keine Synchronisation – wird von gesicherten Methoden aufgerufen.
        /// </summary>
        /// <param name="place">Relative Position.</param>
        /// <param name="c">Zeichen.</param>
        public void _OutTextXY(Point place, char c) => _OutTextXY(place.X, place.Y, c);

        /// <summary>
        /// Thread-sichere Ausgabe eines Strings an relativen Koordinaten.
        /// </summary>
        /// <param name="x">Relative X-Koordinate.</param>
        /// <param name="y">Relative Y-Koordinate.</param>
        /// <param name="s">Auszugebender Text.</param>
        public void OutTextXY(int x, int y, string s)
        {
            lock (this)
                _OutTextXY(x, y, s);
        }

        /// <summary>
        /// Thread-sichere Ausgabe eines Strings an relativen Koordinaten.
        /// </summary>
        /// <param name="x">Relative X-Koordinate.</param>
        /// <param name="y">Relative Y-Koordinate.</param>
        /// <param name="s">Auszugebender Text.</param>
        public void OutTextXY(int x, int y, string s, ConsoleColor f, ConsoleColor b)
        {
            lock (this)
                _OutTextXY(x, y, s,f,b);
        }

        /// <summary>
        /// Nicht-thread-sichere interne Implementierung der String-Ausgabe.
        /// Fügt den Canvas-Offset hinzu und positioniert den Cursor global.
        /// </summary>
        /// <param name="x">Relative X-Koordinate.</param>
        /// <param name="y">Relative Y-Koordinate.</param>
        /// <param name="s">Auszugebender Text.</param>
        private void _OutTextXY(int x, int y, string s)
        {
            _console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
            _console.Write(s);
        }

        /// <summary>
        /// Nicht-thread-sichere interne Implementierung der String-Ausgabe.
        /// Fügt den Canvas-Offset hinzu und positioniert den Cursor global.
        /// </summary>
        /// <param name="x">Relative X-Koordinate.</param>
        /// <param name="y">Relative Y-Koordinate.</param>
        /// <param name="s">Auszugebender Text.</param>
        private void _OutTextXY(int x, int y, string s, ConsoleColor f, ConsoleColor b)
        {
            _console.ForegroundColor = f;
            _console.BackgroundColor = b;
            _console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
            _console.Write(s);
        }

        /// <summary>
        /// Thread-sichere Ausgabe eines Zeichens an relativen Koordinaten.
        /// </summary>
        /// <param name="x">Relative X-Koordinate.</param>
        /// <param name="y">Relative Y-Koordinate.</param>
        /// <param name="c">Auszugebendes Zeichen.</param>
        public void OutTextXY(int x, int y, char c)
        {
            lock (this)
                _OutTextXY(x, y, c);
        }

        /// <summary>
        /// Nicht-thread-sichere interne Implementierung der Zeichenausgabe.
        /// </summary>
        /// <param name="x">Relative X-Koordinate.</param>
        /// <param name="y">Relative Y-Koordinate.</param>
        /// <param name="c">Auszugebendes Zeichen.</param>
        private void _OutTextXY(int x, int y, char c)
        {
            _console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
            _console.Write(c);
        }

        /// <summary>
        /// Thread-sichere Ausgabe eines Zeichens mit expliziten Farben.
        /// Setzt temporär Foreground/Background der Konsole (keine Wiederherstellung
        /// des vorherigen Zustands – Aufrufer ist verantwortlich, falls nötig).
        /// </summary>
        /// <param name="x">Relative X-Koordinate.</param>
        /// <param name="y">Relative Y-Koordinate.</param>
        /// <param name="c">Zeichen.</param>
        /// <param name="f">Vordergrundfarbe.</param>
        /// <param name="b">Hintergrundfarbe.</param>
        public void OutTextXY(int x, int y, char c, ConsoleColor f, ConsoleColor b)
        {
            lock (this)
            {
                _console.ForegroundColor = f;
                _console.BackgroundColor = b;
                _console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
                _console.Write(c);
            }
        }

        /// <summary>
        /// Setzt die Breite und Höhe (Width/Height) der Dimension neu.
        /// Der Offset (X/Y) bleibt unverändert.
        /// Hinweis: Keine Validierung auf Mindestgrößen.
        /// </summary>
        /// <param name="x">Neue Breite.</param>
        /// <param name="y">Neue Höhe.</param>
        public void SetDimension(int x, int y)
            => (_dimension.Width, _dimension.Height) = (x, y);
    }
}
