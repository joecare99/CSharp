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
                var canvasRect = new Rectangle(0, 0, _dimension.Width, _dimension.Height);
                var target = Rectangle.Intersect(canvasRect, dimension);
                if (target.Width <= 0 || target.Height <= 0)
                    return;

                _console.BackgroundColor = bkcolor;
                _console.ForegroundColor = frcolor;

                string line = new string(c, target.Width);
                for (int y = target.Top; y < target.Bottom; y++)
                    _OutTextXY(target.Left, y, line);
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

                var canvasRect = new Rectangle(0, 0, _dimension.Width, _dimension.Height);
                if (!canvasRect.IntersectsWith(dimension)) return;

                _console.BackgroundColor = bkcolor;
                _console.ForegroundColor = frcolor;

                if (dimension.Width == 1)
                {
                    int x = dimension.Left;
                    if (x < canvasRect.Left || x >= canvasRect.Right) return;
                    int startY = Math.Max(dimension.Top, canvasRect.Top);
                    int endY = Math.Min(dimension.Bottom, canvasRect.Bottom);
                    for (int y = startY; y < endY; y++)
                        _OutTextXY(x, y, boarder[1]);
                    return;
                }

                if (dimension.Height == 1)
                {
                    int y = dimension.Top;
                    if (y < canvasRect.Top || y >= canvasRect.Bottom) return;
                    int startX = Math.Max(dimension.Left, canvasRect.Left);
                    int endX = Math.Min(dimension.Right, canvasRect.Right);
                    if (startX >= endX) return;
                    _OutTextXY(startX, y, new string(boarder[0], endX - startX));
                    return;
                }

                int left = dimension.Left;
                int right = dimension.Right - 1;
                int top = dimension.Top;
                int bottom = dimension.Bottom - 1;

                int horizStart = Math.Max(left + 1, canvasRect.Left);
                int horizEnd = Math.Min(right - 1, canvasRect.Right - 1);
                if (horizStart <= horizEnd)
                {
                    string hLine = new string(boarder[0], horizEnd - horizStart + 1);
                    if (top >= canvasRect.Top && top < canvasRect.Bottom)
                        _OutTextXY(horizStart, top, hLine);
                    if (bottom >= canvasRect.Top && bottom < canvasRect.Bottom)
                        _OutTextXY(horizStart, bottom, hLine);
                }

                int vertStart = Math.Max(top + 1, canvasRect.Top);
                int vertEnd = Math.Min(bottom - 1, canvasRect.Bottom - 1);
                if (vertStart <= vertEnd)
                {
                    if (left >= canvasRect.Left && left < canvasRect.Right)
                        for (int y = vertStart; y <= vertEnd; y++)
                            _OutTextXY(left, y, boarder[1]);
                    if (right >= canvasRect.Left && right < canvasRect.Right)
                        for (int y = vertStart; y <= vertEnd; y++)
                            _OutTextXY(right, y, boarder[1]);
                }

                DrawCorner(left, top, boarder[2]);
                DrawCorner(right, top, boarder[3]);
                DrawCorner(left, bottom, boarder[4]);
                DrawCorner(right, bottom, boarder[5]);
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

        private void DrawCorner(int x, int y, char ch)
        {
            if (x >= 0 && x < _dimension.Width && y >= 0 && y < _dimension.Height)
            {
                _OutTextXY(x, y, ch);
            }
        }
    }
}
