using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace CreateCards.Model
{
    public enum CardValues
    {
        Ace, _2, _3, _4, _5, _6, _7, _8, _9, _10, Jack, Queen, King
    }

    public class Card
    {
        public static Dictionary<CardValues, CardDrawDef> DrawDef = new() {
            { CardValues.Ace ,new("A",0.8){  
                PntSuits=new[]{new PointF(0.5f,0.5f) } } },
            { CardValues._2 ,new("2"){  
                PntSuits=new[]{new PointF(0.5f,0.3f), new PointF(0.5f, 0.7f) } } },
            { CardValues._3 ,new("3"){
                PntSuits=new[]{new PointF(0.7f,0.3f), new PointF(0.3f, 0.7f), new PointF(0.5f, 0.5f) } } },
            { CardValues._4 ,new("4"){  
                PntSuits=new[]{ new PointF(0.3f, 0.3f), new PointF(0.7f, 0.7f), new PointF(0.3f, 0.7f), new PointF(0.7f, 0.3f) } } },
            { CardValues._5 ,new("5"){ 
                PntSuits=new[]{ new PointF(0.3f, 0.3f), new PointF(0.7f, 0.7f), new PointF(0.3f, 0.7f), new PointF(0.7f, 0.3f),
                    new PointF(0.5f, 0.5f) } } },
            { CardValues._6 ,new("6"){  
                PntSuits=new[]{ new PointF(0.3f, 0.3f), new PointF(0.7f, 0.7f), new PointF(0.3f, 0.7f), new PointF(0.7f, 0.3f),
                    new PointF(0.3f, 0.5f), new PointF(0.7f, 0.5f) } } },
            { CardValues._7 ,new("7"){  
                PntSuits=new[]{ new PointF(0.3f, 0.25f), new PointF(0.7f, 0.75f), new PointF(0.3f, 0.75f), new PointF(0.7f, 0.25f),
                    new PointF(0.3f, 0.5f), new PointF(0.7f, 0.5f), new PointF(0.5f, 0.375f) } } },
            { CardValues._8 ,new("8"){  
                PntSuits=new[]{ new PointF(0.3f, 0.25f), new PointF(0.7f, 0.75f), new PointF(0.3f, 0.75f), new PointF(0.7f, 0.25f),
                    new PointF(0.3f, 0.5f), new PointF(0.7f, 0.5f), new PointF(0.5f, 0.375f), new PointF(0.5f, 0.625f) } } },
            { CardValues._9 ,new("9"){ 
                PntSuits=new[]{ new PointF(0.3f, 0.25f), new PointF(0.7f, 0.75f), new PointF(0.3f, 0.75f), new PointF(0.7f, 0.25f),
                    new PointF(0.3f, 0.5f), new PointF(0.7f, 0.5f), new PointF(0.5f, 0.25f), new PointF(0.5f, 0.75f), new PointF(0.5f, 0.5f) } } },
            { CardValues._10 ,new("10"){ 
                PntSuits=new[]{ new PointF(0.3f, 0.25f), new PointF(0.7f, 0.75f), new PointF(0.3f, 0.75f), new PointF(0.7f, 0.25f),
                                new PointF(0.3f, 0.375f), new PointF(0.7f, 0.375f), new PointF(0.3f, 0.625f), new PointF(0.7f, 0.625f),
                                new PointF(0.5f, 0.3125f),new PointF(0.5f, 0.6825f) } } },
            { CardValues.Jack ,new("J"){
                CustomDraw = (gfx, rect, color, suit) => DrawJack(gfx, rect, color, suit)
            }},
            { CardValues.Queen ,new("Q"){ 
                CustomDraw = (gfx, rect, color, suit) => DrawQueen(gfx, rect, color, suit)
            }},
            { CardValues.King ,new("K"){ 
                CustomDraw = (gfx, rect, color, suit) => DrawKing(gfx, rect, color, suit)
            }},
        };
        public string Suit { get; set; }
        public CardValues Value { get; set; }
        public Color Color { get; set; }

        public Card(string suit, CardValues value, Color color)
        {
            Suit = suit;
            Value = value;
            Color = color;
        }

        public Bitmap GetCard(int width, int height)
        {
            Bitmap cardBitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(cardBitmap);

            DrawCard(width, height, g);

            return cardBitmap;
        }

        public void DrawCard(int width, int height, Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // Draw card background
            Rectangle rect = new Rectangle(width / 200, width / 200, width - 1 - width / 200, height - 1 - width / 200);
            g.FillRectangle(Brushes.Transparent, rect);
            GraphicsPath path = RoundedRectangle(rect, width / 8);
            g.FillPath(new SolidBrush(Color.White), path);
            g.DrawPath(new Pen(Color.Black, width / 100), path);

            if (DrawDef.TryGetValue(Value, out var drawDef))
            {
                if (drawDef.CustomDraw != null)
                {
                    // Face card or custom layout
                    drawDef.CustomDraw(g, rect, Color, Suit);

                    foreach (var pnt in drawDef.PntVals)
                        DrawText(rect, drawDef.PrintValue, drawDef.ValSize, pnt, g);
                    foreach (var pnt in drawDef.PntSVals)
                        DrawText(rect, Suit, drawDef.ValSize, pnt, g);
                }
                else
                {
                    foreach (var pnt in drawDef.PntVals)
                        DrawText(rect, drawDef.PrintValue, drawDef.ValSize, pnt, g);
                    foreach (var pnt in drawDef.PntSVals)
                        DrawText(rect, Suit, drawDef.ValSize, pnt, g);
                    foreach (var pnt in drawDef.PntSuits)
                        DrawText(rect, Suit, drawDef.SuitSize, pnt, g);
                }
            }

            void DrawText(Rectangle rect, string sP, double fSize, PointF fpOffs, Graphics g)
            {
                int width = rect.Width;
                RectangleF rect2 = rect;
                rect2.Offset(new PointF((fpOffs.X - 0.5f) * rect2.Width, (fpOffs.Y - 0.5f) * rect2.Height));
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                Font font = new Font("Times New Roman", (float)(width * fSize));
                g.DrawString(sP, font, new SolidBrush(Color), rect2, format);
            }
        }

        private static void DrawJack(Graphics g, Rectangle rect, Color color, string suit)
        {
            // French-style Jack (Valet) - elaborate mirrored double-headed design
            float penW = System.Math.Max(1f, rect.Width / 120f);
            using var outlinePen = new Pen(Color.FromArgb(50, 30, 15), penW);
            using var thinPen = new Pen(Color.FromArgb(50, 30, 15), penW * 0.5f);
            using var shadowPen = new Pen(Color.FromArgb(80, 0, 0, 0), penW * 0.4f);

            // Colors - rich palette
            Color cream = Color.FromArgb(255, 252, 245);
            Color gold = Color.FromArgb(212, 175, 55);
            Color darkGold = Color.FromArgb(170, 135, 30);
            Color lightGold = Color.FromArgb(255, 223, 120);
            Color skinTone = Color.FromArgb(255, 220, 185);
            Color skinShadow = Color.FromArgb(235, 190, 150);
            Color hairColor = Color.FromArgb(120, 75, 35);
            Color hairHighlight = Color.FromArgb(160, 110, 60);

            // Draw decorative frame with gradient effect
            RectangleF frameRect = new RectangleF(rect.X + rect.Width * 0.10f, rect.Y + rect.Height * 0.06f,
                                                   rect.Width * 0.80f, rect.Height * 0.88f);

            // Ornate frame background with subtle pattern
            using var frameBrush = new LinearGradientBrush(frameRect, cream, Color.FromArgb(250, 245, 235), 45f);
            g.FillRectangle(frameBrush, frameRect);

            // Draw diagonal pattern in frame
            using var patternPen = new Pen(Color.FromArgb(15, color), penW * 0.3f);
            for (float i = frameRect.X; i < frameRect.Right + frameRect.Height; i += rect.Width * 0.04f)
            {
                g.DrawLine(patternPen, i, frameRect.Y, i - frameRect.Height * 0.3f, frameRect.Y + frameRect.Height * 0.3f);
            }

            // Inner decorative border - double line with gold
            float borderInset = rect.Width * 0.02f;
            RectangleF innerFrame = RectangleF.Inflate(frameRect, -borderInset * 2, -borderInset * 2);
            using var borderPen = new Pen(color, penW * 1.2f);
            using var goldBorderPen = new Pen(gold, penW * 0.6f);
            g.DrawRectangle(borderPen, innerFrame.X, innerFrame.Y, innerFrame.Width, innerFrame.Height);
            g.DrawRectangle(goldBorderPen, innerFrame.X - penW, innerFrame.Y - penW, innerFrame.Width + penW * 2, innerFrame.Height + penW * 2);

            // Corner ornaments
            DrawCornerOrnament(g, innerFrame.X, innerFrame.Y, rect.Width * 0.08f, gold, outlinePen, false, false);
            DrawCornerOrnament(g, innerFrame.Right, innerFrame.Y, rect.Width * 0.08f, gold, outlinePen, true, false);
            DrawCornerOrnament(g, innerFrame.X, innerFrame.Bottom, rect.Width * 0.08f, gold, outlinePen, false, true);
            DrawCornerOrnament(g, innerFrame.Right, innerFrame.Bottom, rect.Width * 0.08f, gold, outlinePen, true, true);

            // Draw upper half figure
            DrawJackHalf(g, innerFrame, color, suit, false, outlinePen, thinPen, shadowPen, 
                skinTone, skinShadow, hairColor, hairHighlight, gold, darkGold, lightGold);

            // Draw lower half figure (mirrored)
            DrawJackHalf(g, innerFrame, color, suit, true, outlinePen, thinPen, shadowPen,
                skinTone, skinShadow, hairColor, hairHighlight, gold, darkGold, lightGold);

            // Center medallion with suit symbol
            float centerY = innerFrame.Y + innerFrame.Height / 2;
            float medalSize = innerFrame.Width * 0.22f;
            RectangleF medalRect = new RectangleF(innerFrame.X + innerFrame.Width / 2 - medalSize / 2, 
                                                   centerY - medalSize / 2, medalSize, medalSize);

            // Medallion with gradient
            using var medalGradient = new LinearGradientBrush(medalRect, Color.White, Color.FromArgb(240, 235, 220), 135f);
            g.FillEllipse(medalGradient, medalRect);
            using var medalBorderPen = new Pen(gold, penW * 1.5f);
            g.DrawEllipse(medalBorderPen, medalRect);
            g.DrawEllipse(outlinePen, RectangleF.Inflate(medalRect, penW, penW));

            // Suit symbol in center
            using var suitFont = new Font("Segoe UI Symbol", medalSize * 0.5f, FontStyle.Bold);
            using var suitBrush = new SolidBrush(color);
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(suit, suitFont, suitBrush, medalRect, sf);
        }

        private static void DrawCornerOrnament(Graphics g, float x, float y, float size, Color gold, Pen outlinePen, bool flipH, bool flipV)
        {
            var state = g.Save();
            g.TranslateTransform(x, y);
            if (flipH) g.ScaleTransform(-1, 1);
            if (flipV) g.ScaleTransform(1, -1);

            using var goldBrush = new SolidBrush(gold);
            // Fleur-de-lis style corner
            PointF[] ornament = {
                new PointF(0, 0),
                new PointF(size * 0.3f, 0),
                new PointF(size * 0.15f, size * 0.15f),
                new PointF(size * 0.4f, size * 0.1f),
                new PointF(size * 0.2f, size * 0.3f),
                new PointF(size * 0.1f, size * 0.4f),
                new PointF(size * 0.15f, size * 0.15f),
                new PointF(0, size * 0.3f)
            };
            g.FillPolygon(goldBrush, ornament);
            g.DrawPolygon(outlinePen, ornament);

            g.Restore(state);
        }

        private static void DrawJackHalf(Graphics g, RectangleF rect, Color color, string suit, bool mirrored,
            Pen outlinePen, Pen thinPen, Pen shadowPen, Color skinTone, Color skinShadow, 
            Color hairColor, Color hairHighlight, Color gold, Color darkGold, Color lightGold)
        {
            var state = g.Save();

            if (mirrored)
            {
                g.TranslateTransform(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                g.RotateTransform(180);
                g.TranslateTransform(-(rect.X + rect.Width / 2), -(rect.Y + rect.Height / 2));
            }

            float halfH = rect.Height * 0.47f;
            RectangleF half = new RectangleF(rect.X, rect.Y, rect.Width, halfH);

            // Shadow layer for depth
            using var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0));

            // Tunic with gradient and folds
            RectangleF tunicRect = new RectangleF(half.X + half.Width * 0.18f, half.Y + half.Height * 0.4f,
                                                   half.Width * 0.64f, half.Height * 0.6f);
            using var tunicGradient = new LinearGradientBrush(tunicRect, 
                Color.FromArgb(255, color), Color.FromArgb(200, ControlPaint.Dark(color, 0.2f)), 0f);

            PointF[] tunic = {
                new PointF(half.X + half.Width * 0.22f, half.Bottom),
                new PointF(half.X + half.Width * 0.18f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.28f, half.Y + half.Height * 0.46f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.42f),
                new PointF(half.X + half.Width * 0.72f, half.Y + half.Height * 0.46f),
                new PointF(half.X + half.Width * 0.82f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.78f, half.Bottom)
            };
            g.FillPolygon(tunicGradient, tunic);
            g.DrawPolygon(outlinePen, tunic);

            // Tunic folds/creases for texture
            using var foldPen = new Pen(Color.FromArgb(60, 0, 0, 0), thinPen.Width);
            using var highlightPen = new Pen(Color.FromArgb(80, 255, 255, 255), thinPen.Width);
            g.DrawLine(foldPen, half.X + half.Width * 0.35f, half.Y + half.Height * 0.55f, 
                       half.X + half.Width * 0.32f, half.Bottom);
            g.DrawLine(highlightPen, half.X + half.Width * 0.37f, half.Y + half.Height * 0.55f, 
                       half.X + half.Width * 0.35f, half.Bottom);
            g.DrawLine(foldPen, half.X + half.Width * 0.65f, half.Y + half.Height * 0.55f, 
                       half.X + half.Width * 0.68f, half.Bottom);

            // Decorative trim on tunic
            using var trimPen = new Pen(gold, outlinePen.Width * 1.2f);
            g.DrawLine(trimPen, half.X + half.Width * 0.5f, half.Y + half.Height * 0.45f,
                       half.X + half.Width * 0.5f, half.Bottom);

            // Elaborate collar with lace detail
            using var collarBrush = new SolidBrush(Color.White);
            PointF[] collar = {
                new PointF(half.X + half.Width * 0.35f, half.Y + half.Height * 0.46f),
                new PointF(half.X + half.Width * 0.42f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.55f),
                new PointF(half.X + half.Width * 0.58f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.65f, half.Y + half.Height * 0.46f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.42f)
            };
            g.FillPolygon(collarBrush, collar);
            g.DrawPolygon(thinPen, collar);

            // Lace pattern on collar
            using var lacePen = new Pen(Color.FromArgb(100, 150, 150, 150), thinPen.Width * 0.5f);
            for (float lx = half.X + half.Width * 0.4f; lx < half.X + half.Width * 0.6f; lx += half.Width * 0.03f)
            {
                g.DrawLine(lacePen, lx, half.Y + half.Height * 0.48f, lx + half.Width * 0.015f, half.Y + half.Height * 0.52f);
            }

            // Head with shading
            float headW = half.Width * 0.24f;
            float headH = half.Height * 0.34f;
            RectangleF headRect = new RectangleF(half.X + half.Width * 0.5f - headW / 2,
                                                  half.Y + half.Height * 0.10f, headW, headH);

            // Hair with texture
            using var hairGradient = new LinearGradientBrush(
                new RectangleF(headRect.X - headW * 0.15f, headRect.Y - headH * 0.1f, headW * 1.3f, headH * 0.6f),
                hairHighlight, hairColor, 45f);
            RectangleF hairRect = new RectangleF(headRect.X - headW * 0.12f, headRect.Y - headH * 0.08f,
                                                  headW * 1.24f, headH * 0.55f);
            g.FillEllipse(hairGradient, hairRect);

            // Hair strands
            using var strandPen = new Pen(Color.FromArgb(80, 60, 40, 20), thinPen.Width * 0.6f);
            for (int i = 0; i < 5; i++)
            {
                float sx = headRect.X + headW * (0.1f + i * 0.2f);
                g.DrawBezier(strandPen, 
                    new PointF(sx, headRect.Y), 
                    new PointF(sx - headW * 0.05f, headRect.Y + headH * 0.1f),
                    new PointF(sx + headW * 0.05f, headRect.Y + headH * 0.2f),
                    new PointF(sx, headRect.Y + headH * 0.3f));
            }

            // Face with gradient for 3D effect
            using var faceGradient = new LinearGradientBrush(headRect, skinTone, skinShadow, 120f);
            g.FillEllipse(faceGradient, headRect);
            g.DrawEllipse(outlinePen, headRect);

            // Cheek blush
            using var blushBrush = new SolidBrush(Color.FromArgb(40, 255, 150, 150));
            g.FillEllipse(blushBrush, headRect.X + headW * 0.6f, headRect.Y + headH * 0.45f, headW * 0.25f, headH * 0.15f);

            // Elaborate beret/cap with feather
            using var capGradient = new LinearGradientBrush(
                new RectangleF(headRect.X - headW * 0.2f, headRect.Y - headH * 0.35f, headW * 1.6f, headH * 0.6f),
                ControlPaint.Light(color, 0.2f), color, 90f);
            PointF[] cap = {
                new PointF(headRect.X - headW * 0.18f, headRect.Y + headH * 0.18f),
                new PointF(headRect.X + headW * 0.1f, headRect.Y - headH * 0.15f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y - headH * 0.28f),
                new PointF(headRect.Right + headW * 0.35f, headRect.Y - headH * 0.12f),
                new PointF(headRect.Right + headW * 0.2f, headRect.Y + headH * 0.22f)
            };
            g.FillPolygon(capGradient, cap);
            g.DrawPolygon(outlinePen, cap);

            // Cap band
            using var bandBrush = new SolidBrush(gold);
            g.FillRectangle(bandBrush, headRect.X - headW * 0.1f, headRect.Y + headH * 0.08f, headW * 1.2f, headH * 0.08f);

            // Jewel on cap
            using var jewelBrush = new SolidBrush(Color.FromArgb(220, 20, 60));
            float jewelSize = headW * 0.12f;
            g.FillEllipse(jewelBrush, headRect.X + headW * 0.4f, headRect.Y + headH * 0.02f, jewelSize, jewelSize);
            g.DrawEllipse(thinPen, headRect.X + headW * 0.4f, headRect.Y + headH * 0.02f, jewelSize, jewelSize);

            // Elaborate feather with multiple strands
            using var featherGradient = new LinearGradientBrush(
                new RectangleF(headRect.Right, headRect.Y - headH * 0.7f, headW * 0.8f, headH * 0.8f),
                lightGold, gold, 45f);
            using var featherPen = new Pen(featherGradient, outlinePen.Width * 2f);

            // Main feather curve
            g.DrawBezier(featherPen,
                new PointF(headRect.Right + headW * 0.25f, headRect.Y - headH * 0.05f),
                new PointF(headRect.Right + headW * 0.6f, headRect.Y - headH * 0.35f),
                new PointF(headRect.Right + headW * 0.5f, headRect.Y - headH * 0.55f),
                new PointF(headRect.Right + headW * 0.15f, headRect.Y - headH * 0.45f));

            // Feather barbs
            using var barbPen = new Pen(gold, thinPen.Width * 0.8f);
            for (int i = 0; i < 6; i++)
            {
                float t = i / 6f;
                float bx = headRect.Right + headW * (0.25f + t * 0.25f);
                float by = headRect.Y - headH * (0.05f + t * 0.35f);
                g.DrawLine(barbPen, bx, by, bx + headW * 0.15f, by - headH * 0.08f);
                g.DrawLine(barbPen, bx, by, bx - headW * 0.1f, by - headH * 0.12f);
            }

            // Eye with detail
            float eyeW = headW * 0.14f;
            float eyeH = headH * 0.08f;
            float eyeX = headRect.X + headW * 0.52f;
            float eyeY = headRect.Y + headH * 0.36f;

            // Eye white
            using var eyeWhiteBrush = new SolidBrush(Color.FromArgb(250, 250, 250));
            g.FillEllipse(eyeWhiteBrush, eyeX, eyeY, eyeW, eyeH);

            // Iris
            using var irisBrush = new SolidBrush(Color.FromArgb(80, 60, 40));
            g.FillEllipse(irisBrush, eyeX + eyeW * 0.3f, eyeY + eyeH * 0.1f, eyeW * 0.5f, eyeH * 0.8f);

            // Pupil
            using var pupilBrush = new SolidBrush(Color.Black);
            g.FillEllipse(pupilBrush, eyeX + eyeW * 0.4f, eyeY + eyeH * 0.25f, eyeW * 0.3f, eyeH * 0.5f);

            // Eye highlight
            using var eyeHighlight = new SolidBrush(Color.White);
            g.FillEllipse(eyeHighlight, eyeX + eyeW * 0.35f, eyeY + eyeH * 0.2f, eyeW * 0.15f, eyeH * 0.3f);
            g.DrawEllipse(thinPen, eyeX, eyeY, eyeW, eyeH);

            // Eyebrow
            using var browPen = new Pen(hairColor, thinPen.Width * 1.2f);
            g.DrawArc(browPen, eyeX - eyeW * 0.1f, eyeY - eyeH * 0.8f, eyeW * 1.2f, eyeH, 10, 70);

            // Nose with shadow
            g.DrawLine(thinPen,
                new PointF(headRect.X + headW * 0.62f, headRect.Y + headH * 0.42f),
                new PointF(headRect.X + headW * 0.68f, headRect.Y + headH * 0.55f));
            g.DrawArc(thinPen, headRect.X + headW * 0.58f, headRect.Y + headH * 0.52f, headW * 0.12f, headH * 0.08f, 30, 120);

            // Mouth
            using var lipPen = new Pen(Color.FromArgb(180, 100, 90), thinPen.Width);
            g.DrawArc(lipPen, headRect.X + headW * 0.52f, headRect.Y + headH * 0.62f, headW * 0.2f, headH * 0.1f, 0, 180);

            // Halberd with detailed blade
            float shaftX = half.X + half.Width * 0.10f;

            // Shaft with wood grain
            RectangleF shaftRect = new RectangleF(shaftX, half.Y + half.Height * 0.08f, half.Width * 0.028f, half.Height * 0.88f);
            using var shaftGradient = new LinearGradientBrush(shaftRect, 
                Color.FromArgb(160, 110, 60), Color.FromArgb(100, 65, 30), 0f);
            g.FillRectangle(shaftGradient, shaftRect);
            g.DrawRectangle(outlinePen, shaftRect.X, shaftRect.Y, shaftRect.Width, shaftRect.Height);

            // Wood grain lines
            using var grainPen = new Pen(Color.FromArgb(50, 60, 40, 20), thinPen.Width * 0.3f);
            for (float gy = shaftRect.Y; gy < shaftRect.Bottom; gy += half.Height * 0.05f)
            {
                g.DrawLine(grainPen, shaftRect.X + shaftRect.Width * 0.2f, gy, 
                           shaftRect.X + shaftRect.Width * 0.8f, gy + half.Height * 0.02f);
            }

            // Elaborate blade with engraving
            using var bladeGradient = new LinearGradientBrush(
                new RectangleF(shaftX, half.Y, half.Width * 0.15f, half.Height * 0.3f),
                Color.FromArgb(220, 220, 230), Color.FromArgb(160, 165, 175), 90f);
            PointF[] blade = {
                new PointF(shaftX + half.Width * 0.028f, half.Y + half.Height * 0.12f),
                new PointF(shaftX + half.Width * 0.08f, half.Y + half.Height * 0.04f),
                new PointF(shaftX + half.Width * 0.14f, half.Y + half.Height * 0.06f),
                new PointF(shaftX + half.Width * 0.12f, half.Y + half.Height * 0.18f),
                new PointF(shaftX + half.Width * 0.08f, half.Y + half.Height * 0.28f),
                new PointF(shaftX + half.Width * 0.028f, half.Y + half.Height * 0.24f)
            };
            g.FillPolygon(bladeGradient, blade);
            g.DrawPolygon(outlinePen, blade);

            // Blade edge highlight
            using var edgePen = new Pen(Color.FromArgb(150, 255, 255, 255), thinPen.Width * 0.5f);
            g.DrawLine(edgePen, blade[1], blade[2]);

            // Axe spike
            PointF[] spike = {
                new PointF(shaftX + half.Width * 0.014f, half.Y + half.Height * 0.08f),
                new PointF(shaftX + half.Width * 0.014f, half.Y),
                new PointF(shaftX + half.Width * 0.028f, half.Y + half.Height * 0.08f)
            };
            g.FillPolygon(bladeGradient, spike);
            g.DrawPolygon(outlinePen, spike);

            g.Restore(state);
        }

        private static void DrawQueen(Graphics g, Rectangle rect, Color color, string suit)
        {
            // French-style Queen (Dame) - elaborate mirrored double-headed design
            float penW = System.Math.Max(1f, rect.Width / 120f);
            using var outlinePen = new Pen(Color.FromArgb(50, 30, 15), penW);
            using var thinPen = new Pen(Color.FromArgb(50, 30, 15), penW * 0.5f);

            // Colors - elegant palette
            Color cream = Color.FromArgb(255, 252, 245);
            Color gold = Color.FromArgb(212, 175, 55);
            Color lightGold = Color.FromArgb(255, 223, 120);
            Color skinTone = Color.FromArgb(255, 225, 200);
            Color skinShadow = Color.FromArgb(240, 200, 175);
            Color hairColor = Color.FromArgb(180, 140, 60);
            Color hairHighlight = Color.FromArgb(220, 190, 100);

            // Draw decorative frame with gradient
            RectangleF frameRect = new RectangleF(rect.X + rect.Width * 0.10f, rect.Y + rect.Height * 0.06f,
                                                   rect.Width * 0.80f, rect.Height * 0.88f);
            using var frameBrush = new LinearGradientBrush(frameRect, cream, Color.FromArgb(250, 245, 235), 45f);
            g.FillRectangle(frameBrush, frameRect);

            // Elegant floral pattern in background
            using var patternPen = new Pen(Color.FromArgb(12, color), penW * 0.4f);
            for (float px = frameRect.X; px < frameRect.Right; px += rect.Width * 0.06f)
            {
                for (float py = frameRect.Y; py < frameRect.Bottom; py += rect.Height * 0.05f)
                {
                    g.DrawEllipse(patternPen, px, py, rect.Width * 0.02f, rect.Width * 0.02f);
                }
            }

            // Inner decorative border - double line
            float borderInset = rect.Width * 0.02f;
            RectangleF innerFrame = RectangleF.Inflate(frameRect, -borderInset * 2, -borderInset * 2);
            using var borderPen = new Pen(color, penW * 1.2f);
            using var goldBorderPen = new Pen(gold, penW * 0.6f);
            g.DrawRectangle(borderPen, innerFrame.X, innerFrame.Y, innerFrame.Width, innerFrame.Height);
            g.DrawRectangle(goldBorderPen, innerFrame.X - penW, innerFrame.Y - penW, innerFrame.Width + penW * 2, innerFrame.Height + penW * 2);

            // Floral corner ornaments
            DrawFloralCorner(g, innerFrame.X, innerFrame.Y, rect.Width * 0.1f, gold, color, outlinePen, false, false);
            DrawFloralCorner(g, innerFrame.Right, innerFrame.Y, rect.Width * 0.1f, gold, color, outlinePen, true, false);
            DrawFloralCorner(g, innerFrame.X, innerFrame.Bottom, rect.Width * 0.1f, gold, color, outlinePen, false, true);
            DrawFloralCorner(g, innerFrame.Right, innerFrame.Bottom, rect.Width * 0.1f, gold, color, outlinePen, true, true);

            // Draw upper half figure
            DrawQueenHalf(g, innerFrame, color, suit, false, outlinePen, thinPen, skinTone, skinShadow, hairColor, hairHighlight, gold, lightGold);

            // Draw lower half figure (mirrored)
            DrawQueenHalf(g, innerFrame, color, suit, true, outlinePen, thinPen, skinTone, skinShadow, hairColor, hairHighlight, gold, lightGold);

            // Center medallion with suit symbol
            float centerY = innerFrame.Y + innerFrame.Height / 2;
            float medalSize = innerFrame.Width * 0.24f;
            RectangleF medalRect = new RectangleF(innerFrame.X + innerFrame.Width / 2 - medalSize / 2,
                                                   centerY - medalSize / 2, medalSize, medalSize);

            // Ornate medallion
            using var medalGradient = new LinearGradientBrush(medalRect, Color.White, Color.FromArgb(245, 240, 230), 135f);
            g.FillEllipse(medalGradient, medalRect);

            // Decorative ring around medallion
            using var ringPen = new Pen(gold, penW * 2f);
            g.DrawEllipse(ringPen, medalRect);
            using var innerRingPen = new Pen(color, penW * 0.8f);
            g.DrawEllipse(innerRingPen, RectangleF.Inflate(medalRect, -penW * 2, -penW * 2));

            // Suit symbol
            using var suitFont = new Font("Segoe UI Symbol", medalSize * 0.45f, FontStyle.Bold);
            using var suitBrush = new SolidBrush(color);
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(suit, suitFont, suitBrush, medalRect, sf);
        }

        private static void DrawFloralCorner(Graphics g, float x, float y, float size, Color gold, Color accent, Pen outlinePen, bool flipH, bool flipV)
        {
            var state = g.Save();
            g.TranslateTransform(x, y);
            if (flipH) g.ScaleTransform(-1, 1);
            if (flipV) g.ScaleTransform(1, -1);

            using var goldBrush = new SolidBrush(gold);
            using var accentBrush = new SolidBrush(accent);

            // Main curved vine
            using var vinePen = new Pen(gold, outlinePen.Width * 1.2f);
            g.DrawBezier(vinePen,
                new PointF(0, size * 0.5f),
                new PointF(size * 0.2f, size * 0.3f),
                new PointF(size * 0.3f, size * 0.2f),
                new PointF(size * 0.5f, 0));

            // Small leaves
            PointF[] leaf1 = {
                new PointF(size * 0.15f, size * 0.35f),
                new PointF(size * 0.05f, size * 0.15f),
                new PointF(size * 0.25f, size * 0.25f)
            };
            g.FillPolygon(goldBrush, leaf1);

            PointF[] leaf2 = {
                new PointF(size * 0.35f, size * 0.15f),
                new PointF(size * 0.15f, size * 0.05f),
                new PointF(size * 0.25f, size * 0.25f)
            };
            g.FillPolygon(goldBrush, leaf2);

            // Small flower
            float flowerX = size * 0.08f;
            float flowerY = size * 0.08f;
            float petalR = size * 0.06f;
            g.FillEllipse(accentBrush, flowerX - petalR, flowerY - petalR * 1.5f, petalR * 2, petalR * 2);
            g.FillEllipse(accentBrush, flowerX - petalR * 1.5f, flowerY - petalR, petalR * 2, petalR * 2);
            g.FillEllipse(goldBrush, flowerX - petalR * 0.5f, flowerY - petalR * 0.5f, petalR, petalR);

            g.Restore(state);
        }

        private static void DrawQueenHalf(Graphics g, RectangleF rect, Color color, string suit, bool mirrored,
            Pen outlinePen, Pen thinPen, Color skinTone, Color skinShadow, Color hairColor, Color hairHighlight, Color gold, Color lightGold)
        {
            var state = g.Save();

            if (mirrored)
            {
                g.TranslateTransform(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                g.RotateTransform(180);
                g.TranslateTransform(-(rect.X + rect.Width / 2), -(rect.Y + rect.Height / 2));
            }

            float halfH = rect.Height * 0.47f;
            RectangleF half = new RectangleF(rect.X, rect.Y, rect.Width, halfH);

            // Elaborate gown with gradient and layers
            RectangleF gownRect = new RectangleF(half.X + half.Width * 0.12f, half.Y + half.Height * 0.42f,
                                                  half.Width * 0.76f, half.Height * 0.58f);
            using var gownGradient = new LinearGradientBrush(gownRect,
                Color.FromArgb(255, color), Color.FromArgb(200, ControlPaint.Dark(color, 0.15f)), 90f);

            // Full gown shape
            PointF[] gown = {
                new PointF(half.X + half.Width * 0.12f, half.Bottom),
                new PointF(half.X + half.Width * 0.15f, half.Y + half.Height * 0.58f),
                new PointF(half.X + half.Width * 0.22f, half.Y + half.Height * 0.5f),
                new PointF(half.X + half.Width * 0.32f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.42f),
                new PointF(half.X + half.Width * 0.68f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.78f, half.Y + half.Height * 0.5f),
                new PointF(half.X + half.Width * 0.85f, half.Y + half.Height * 0.58f),
                new PointF(half.X + half.Width * 0.88f, half.Bottom)
            };
            g.FillPolygon(gownGradient, gown);
            g.DrawPolygon(outlinePen, gown);

            // Gown folds for depth
            using var foldPen = new Pen(Color.FromArgb(50, 0, 0, 0), thinPen.Width);
            using var highlightPen = new Pen(Color.FromArgb(60, 255, 255, 255), thinPen.Width);
            g.DrawBezier(foldPen,
                new PointF(half.X + half.Width * 0.3f, half.Y + half.Height * 0.5f),
                new PointF(half.X + half.Width * 0.28f, half.Y + half.Height * 0.7f),
                new PointF(half.X + half.Width * 0.25f, half.Y + half.Height * 0.85f),
                new PointF(half.X + half.Width * 0.22f, half.Bottom));
            g.DrawBezier(highlightPen,
                new PointF(half.X + half.Width * 0.32f, half.Y + half.Height * 0.5f),
                new PointF(half.X + half.Width * 0.3f, half.Y + half.Height * 0.7f),
                new PointF(half.X + half.Width * 0.28f, half.Y + half.Height * 0.85f),
                new PointF(half.X + half.Width * 0.26f, half.Bottom));
            g.DrawBezier(foldPen,
                new PointF(half.X + half.Width * 0.7f, half.Y + half.Height * 0.5f),
                new PointF(half.X + half.Width * 0.72f, half.Y + half.Height * 0.7f),
                new PointF(half.X + half.Width * 0.75f, half.Y + half.Height * 0.85f),
                new PointF(half.X + half.Width * 0.78f, half.Bottom));

            // Decorative embroidery on gown
            using var embPen = new Pen(gold, thinPen.Width * 1.2f);
            // Vertical ornament
            g.DrawLine(embPen, half.X + half.Width * 0.5f, half.Y + half.Height * 0.5f,
                       half.X + half.Width * 0.5f, half.Bottom);
            // Small diamonds along the line
            for (float ey = half.Y + half.Height * 0.55f; ey < half.Bottom - half.Height * 0.1f; ey += half.Height * 0.1f)
            {
                PointF[] diamond = {
                    new PointF(half.X + half.Width * 0.5f, ey - half.Height * 0.02f),
                    new PointF(half.X + half.Width * 0.52f, ey),
                    new PointF(half.X + half.Width * 0.5f, ey + half.Height * 0.02f),
                    new PointF(half.X + half.Width * 0.48f, ey)
                };
                using var diamondBrush = new SolidBrush(gold);
                g.FillPolygon(diamondBrush, diamond);
            }

            // Elaborate neckline with lace
            using var laceBrush = new SolidBrush(Color.White);
            PointF[] neckLace = {
                new PointF(half.X + half.Width * 0.32f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.38f, half.Y + half.Height * 0.5f),
                new PointF(half.X + half.Width * 0.44f, half.Y + half.Height * 0.53f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.55f),
                new PointF(half.X + half.Width * 0.56f, half.Y + half.Height * 0.53f),
                new PointF(half.X + half.Width * 0.62f, half.Y + half.Height * 0.5f),
                new PointF(half.X + half.Width * 0.68f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.42f)
            };
            g.FillPolygon(laceBrush, neckLace);
            g.DrawPolygon(thinPen, neckLace);

            // Lace scalloped edge
            using var scallopPen = new Pen(Color.FromArgb(120, 120, 120), thinPen.Width * 0.6f);
            for (float sx = half.X + half.Width * 0.36f; sx < half.X + half.Width * 0.64f; sx += half.Width * 0.04f)
            {
                g.DrawArc(scallopPen, sx, half.Y + half.Height * 0.51f, half.Width * 0.04f, half.Height * 0.03f, 0, 180);
            }

            // Necklace with pendant
            using var necklacePen = new Pen(gold, thinPen.Width * 1.5f);
            g.DrawArc(necklacePen, half.X + half.Width * 0.38f, half.Y + half.Height * 0.42f,
                      half.Width * 0.24f, half.Height * 0.12f, 0, 180);

            // Pendant
            using var pendantBrush = new SolidBrush(color);
            using var goldBrush = new SolidBrush(gold);
            float pendX = half.X + half.Width * 0.5f;
            float pendY = half.Y + half.Height * 0.52f;
            g.FillEllipse(goldBrush, pendX - half.Width * 0.025f, pendY, half.Width * 0.05f, half.Width * 0.05f);
            g.FillEllipse(pendantBrush, pendX - half.Width * 0.018f, pendY + half.Width * 0.007f, 
                          half.Width * 0.036f, half.Width * 0.036f);

            // Elaborate hair with curls
            float headW = half.Width * 0.22f;
            float headH = half.Height * 0.3f;
            RectangleF headRect = new RectangleF(half.X + half.Width * 0.5f - headW / 2,
                                                  half.Y + half.Height * 0.12f, headW, headH);

            // Hair base with gradient
            using var hairGradient = new LinearGradientBrush(
                new RectangleF(headRect.X - headW * 0.3f, headRect.Y - headH * 0.15f, headW * 1.6f, headH * 1.2f),
                hairHighlight, hairColor, 60f);

            // Main hair volume
            RectangleF hairOuter = new RectangleF(headRect.X - headW * 0.28f, headRect.Y - headH * 0.12f,
                                                   headW * 1.56f, headH * 1.15f);
            g.FillEllipse(hairGradient, hairOuter);

            // Hair curls on sides
            using var curlPen = new Pen(Color.FromArgb(100, 60, 25), thinPen.Width * 0.8f);
            // Left curls
            g.DrawBezier(curlPen,
                new PointF(headRect.X - headW * 0.15f, headRect.Y + headH * 0.3f),
                new PointF(headRect.X - headW * 0.3f, headRect.Y + headH * 0.5f),
                new PointF(headRect.X - headW * 0.25f, headRect.Y + headH * 0.7f),
                new PointF(headRect.X - headW * 0.1f, headRect.Y + headH * 0.6f));
            g.DrawBezier(curlPen,
                new PointF(headRect.X - headW * 0.1f, headRect.Y + headH * 0.5f),
                new PointF(headRect.X - headW * 0.22f, headRect.Y + headH * 0.65f),
                new PointF(headRect.X - headW * 0.18f, headRect.Y + headH * 0.8f),
                new PointF(headRect.X - headW * 0.05f, headRect.Y + headH * 0.75f));
            // Right curls
            g.DrawBezier(curlPen,
                new PointF(headRect.Right + headW * 0.15f, headRect.Y + headH * 0.3f),
                new PointF(headRect.Right + headW * 0.3f, headRect.Y + headH * 0.5f),
                new PointF(headRect.Right + headW * 0.25f, headRect.Y + headH * 0.7f),
                new PointF(headRect.Right + headW * 0.1f, headRect.Y + headH * 0.6f));

            // Face with 3D shading
            using var faceGradient = new LinearGradientBrush(headRect, skinTone, skinShadow, 130f);
            g.FillEllipse(faceGradient, headRect);
            g.DrawEllipse(outlinePen, headRect);

            // Rosy cheeks
            using var blushBrush = new SolidBrush(Color.FromArgb(35, 255, 130, 130));
            g.FillEllipse(blushBrush, headRect.X + headW * 0.08f, headRect.Y + headH * 0.45f, headW * 0.22f, headH * 0.15f);
            g.FillEllipse(blushBrush, headRect.X + headW * 0.7f, headRect.Y + headH * 0.45f, headW * 0.22f, headH * 0.15f);

            // Elegant tiara crown
            float crownH = headH * 0.45f;
            float crownW = headW * 1.4f;
            float crownX = headRect.X - headW * 0.2f;
            float crownY = headRect.Y - crownH * 0.55f;

            // Crown with gradient
            using var crownGradient = new LinearGradientBrush(
                new RectangleF(crownX, crownY, crownW, crownH), lightGold, gold, 90f);

            PointF[] crown = {
                new PointF(crownX, crownY + crownH),
                new PointF(crownX + crownW * 0.08f, crownY + crownH * 0.35f),
                new PointF(crownX + crownW * 0.18f, crownY + crownH * 0.75f),
                new PointF(crownX + crownW * 0.28f, crownY + crownH * 0.15f),
                new PointF(crownX + crownW * 0.38f, crownY + crownH * 0.6f),
                new PointF(crownX + crownW * 0.5f, crownY - crownH * 0.1f),
                new PointF(crownX + crownW * 0.62f, crownY + crownH * 0.6f),
                new PointF(crownX + crownW * 0.72f, crownY + crownH * 0.15f),
                new PointF(crownX + crownW * 0.82f, crownY + crownH * 0.75f),
                new PointF(crownX + crownW * 0.92f, crownY + crownH * 0.35f),
                new PointF(crownX + crownW, crownY + crownH)
            };
            g.FillPolygon(crownGradient, crown);
            g.DrawPolygon(outlinePen, crown);

            // Crown jewels
            float jewelSize = crownW * 0.065f;
            using var rubyBrush = new SolidBrush(Color.FromArgb(220, 20, 60));
            using var sapphireBrush = new SolidBrush(Color.FromArgb(30, 80, 180));
            using var pearlBrush = new SolidBrush(Color.FromArgb(255, 250, 245));

            // Center jewel (ruby)
            g.FillEllipse(rubyBrush, crownX + crownW * 0.5f - jewelSize * 0.7f, crownY - crownH * 0.1f - jewelSize * 0.3f, 
                          jewelSize * 1.4f, jewelSize * 1.4f);
            g.DrawEllipse(thinPen, crownX + crownW * 0.5f - jewelSize * 0.7f, crownY - crownH * 0.1f - jewelSize * 0.3f, 
                          jewelSize * 1.4f, jewelSize * 1.4f);

            // Side jewels
            g.FillEllipse(sapphireBrush, crownX + crownW * 0.28f - jewelSize/2, crownY + crownH * 0.1f, jewelSize, jewelSize);
            g.FillEllipse(sapphireBrush, crownX + crownW * 0.72f - jewelSize/2, crownY + crownH * 0.1f, jewelSize, jewelSize);

            // Pearls
            g.FillEllipse(pearlBrush, crownX + crownW * 0.08f - jewelSize * 0.4f, crownY + crownH * 0.3f, jewelSize * 0.8f, jewelSize * 0.8f);
            g.FillEllipse(pearlBrush, crownX + crownW * 0.92f - jewelSize * 0.4f, crownY + crownH * 0.3f, jewelSize * 0.8f, jewelSize * 0.8f);

            // Detailed eyes
            float eyeW = headW * 0.12f;
            float eyeH = headH * 0.07f;

            // Left eye
            DrawDetailedEye(g, headRect.X + headW * 0.22f, headRect.Y + headH * 0.38f, eyeW, eyeH, 
                           Color.FromArgb(100, 80, 60), thinPen);
            // Right eye
            DrawDetailedEye(g, headRect.X + headW * 0.66f, headRect.Y + headH * 0.38f, eyeW, eyeH, 
                           Color.FromArgb(100, 80, 60), thinPen);

            // Eyebrows
            using var browPen = new Pen(Color.FromArgb(120, 90, 50), thinPen.Width);
            g.DrawArc(browPen, headRect.X + headW * 0.18f, headRect.Y + headH * 0.28f, eyeW * 1.3f, eyeH, 200, 70);
            g.DrawArc(browPen, headRect.X + headW * 0.62f, headRect.Y + headH * 0.28f, eyeW * 1.3f, eyeH, 250, 70);

            // Elegant nose
            g.DrawBezier(thinPen,
                new PointF(headRect.X + headW * 0.48f, headRect.Y + headH * 0.4f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y + headH * 0.5f),
                new PointF(headRect.X + headW * 0.52f, headRect.Y + headH * 0.55f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y + headH * 0.58f));

            // Lips with detail
            using var lipBrush = new SolidBrush(Color.FromArgb(200, 90, 90));
            using var lipHighlight = new SolidBrush(Color.FromArgb(100, 255, 200, 200));

            // Upper lip
            PointF[] upperLip = {
                new PointF(headRect.X + headW * 0.38f, headRect.Y + headH * 0.68f),
                new PointF(headRect.X + headW * 0.45f, headRect.Y + headH * 0.65f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y + headH * 0.67f),
                new PointF(headRect.X + headW * 0.55f, headRect.Y + headH * 0.65f),
                new PointF(headRect.X + headW * 0.62f, headRect.Y + headH * 0.68f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y + headH * 0.72f)
            };
            g.FillPolygon(lipBrush, upperLip);

            // Lower lip
            g.FillEllipse(lipBrush, headRect.X + headW * 0.4f, headRect.Y + headH * 0.69f, headW * 0.2f, headH * 0.08f);
            g.FillEllipse(lipHighlight, headRect.X + headW * 0.44f, headRect.Y + headH * 0.71f, headW * 0.08f, headH * 0.03f);

            // Elegant flower scepter
            float flowerX = half.X + half.Width * 0.85f;
            float flowerY = half.Y + half.Height * 0.25f;
            DrawElaborateFlower(g, flowerX, flowerY, half.Width * 0.1f, color, gold, outlinePen, thinPen);

            // Flower stem
            using var stemPen = new Pen(Color.FromArgb(80, 120, 60), outlinePen.Width * 1.2f);
            g.DrawBezier(stemPen,
                new PointF(flowerX, flowerY + half.Width * 0.08f),
                new PointF(flowerX - half.Width * 0.02f, flowerY + half.Height * 0.3f),
                new PointF(flowerX + half.Width * 0.02f, flowerY + half.Height * 0.5f),
                new PointF(flowerX - half.Width * 0.01f, half.Bottom));

            // Small leaves on stem
            using var leafBrush = new SolidBrush(Color.FromArgb(100, 140, 80));
            PointF[] leaf1 = {
                new PointF(flowerX, flowerY + half.Height * 0.35f),
                new PointF(flowerX + half.Width * 0.06f, flowerY + half.Height * 0.32f),
                new PointF(flowerX + half.Width * 0.02f, flowerY + half.Height * 0.4f)
            };
            g.FillPolygon(leafBrush, leaf1);

            g.Restore(state);
        }

        private static void DrawDetailedEye(Graphics g, float x, float y, float w, float h, Color irisColor, Pen outlinePen)
        {
            // Eye white
            using var whiteBrush = new SolidBrush(Color.FromArgb(252, 252, 252));
            g.FillEllipse(whiteBrush, x, y, w, h);

            // Iris
            using var irisBrush = new SolidBrush(irisColor);
            float irisW = w * 0.5f;
            float irisH = h * 0.85f;
            g.FillEllipse(irisBrush, x + w * 0.25f, y + h * 0.08f, irisW, irisH);

            // Pupil
            using var pupilBrush = new SolidBrush(Color.Black);
            g.FillEllipse(pupilBrush, x + w * 0.35f, y + h * 0.2f, irisW * 0.6f, irisH * 0.6f);

            // Highlight
            using var highlightBrush = new SolidBrush(Color.White);
            g.FillEllipse(highlightBrush, x + w * 0.32f, y + h * 0.15f, w * 0.12f, h * 0.25f);

            // Eye outline
            g.DrawEllipse(outlinePen, x, y, w, h);

            // Eyelashes
            using var lashPen = new Pen(Color.FromArgb(60, 40, 30), outlinePen.Width * 0.4f);
            for (int i = 0; i < 4; i++)
            {
                float lx = x + w * (0.15f + i * 0.22f);
                g.DrawLine(lashPen, lx, y, lx - w * 0.02f, y - h * 0.15f);
            }
        }

        private static void DrawElaborateFlower(Graphics g, float x, float y, float size, Color petalColor, Color centerColor, Pen outlinePen, Pen thinPen)
        {
            // Outer petals
            using var petalGradient = new LinearGradientBrush(
                new RectangleF(x - size, y - size, size * 2, size * 2),
                ControlPaint.Light(petalColor, 0.3f), petalColor, 45f);

            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60 * (float)System.Math.PI / 180;
                float px = x + (float)System.Math.Cos(angle) * size * 0.7f;
                float py = y + (float)System.Math.Sin(angle) * size * 0.7f;

                g.FillEllipse(petalGradient, px - size * 0.45f, py - size * 0.45f, size * 0.9f, size * 0.9f);
                g.DrawEllipse(thinPen, px - size * 0.45f, py - size * 0.45f, size * 0.9f, size * 0.9f);
            }

            // Inner petals
            for (int i = 0; i < 6; i++)
            {
                float angle = (i * 60 + 30) * (float)System.Math.PI / 180;
                float px = x + (float)System.Math.Cos(angle) * size * 0.4f;
                float py = y + (float)System.Math.Sin(angle) * size * 0.4f;

                using var innerBrush = new SolidBrush(ControlPaint.Light(petalColor, 0.2f));
                g.FillEllipse(innerBrush, px - size * 0.3f, py - size * 0.3f, size * 0.6f, size * 0.6f);
            }

            // Center
            using var centerBrush = new SolidBrush(centerColor);
            g.FillEllipse(centerBrush, x - size * 0.25f, y - size * 0.25f, size * 0.5f, size * 0.5f);
            g.DrawEllipse(thinPen, x - size * 0.25f, y - size * 0.25f, size * 0.5f, size * 0.5f);

            // Center dots
            using var dotBrush = new SolidBrush(Color.FromArgb(180, 140, 40));
            for (int i = 0; i < 5; i++)
            {
                float angle = i * 72 * (float)System.Math.PI / 180;
                float dx = x + (float)System.Math.Cos(angle) * size * 0.12f;
                float dy = y + (float)System.Math.Sin(angle) * size * 0.12f;
                g.FillEllipse(dotBrush, dx - size * 0.04f, dy - size * 0.04f, size * 0.08f, size * 0.08f);
            }
        }

        private static void DrawKing(Graphics g, Rectangle rect, Color color, string suit)
        {
            // French-style King (Roi) - mirrored double-headed design
            float penW = System.Math.Max(1f, rect.Width / 120f);
            using var outlinePen = new Pen(Color.FromArgb(60, 40, 20), penW);
            using var thinPen = new Pen(Color.FromArgb(60, 40, 20), penW * 0.6f);

            // Colors
            Color cream = Color.FromArgb(255, 250, 240);
            Color gold = Color.FromArgb(218, 165, 32);
            Color darkGold = Color.FromArgb(184, 134, 11);
            Color skinTone = Color.FromArgb(255, 218, 185);
            Color beardColor = Color.FromArgb(120, 80, 40);

            // Draw decorative frame
            RectangleF frameRect = new RectangleF(rect.X + rect.Width * 0.12f, rect.Y + rect.Height * 0.08f,
                                                   rect.Width * 0.76f, rect.Height * 0.84f);
            using var frameBrush = new SolidBrush(cream);
            g.FillRectangle(frameBrush, frameRect);

            // Inner decorative border
            float borderInset = rect.Width * 0.02f;
            RectangleF innerFrame = RectangleF.Inflate(frameRect, -borderInset * 2, -borderInset * 2);
            using var borderPen = new Pen(color, penW * 0.8f);
            g.DrawRectangle(borderPen, innerFrame.X, innerFrame.Y, innerFrame.Width, innerFrame.Height);

            // Draw upper half figure
            DrawKingHalf(g, innerFrame, color, suit, false, outlinePen, thinPen, skinTone, beardColor, gold, darkGold);

            // Draw lower half figure (mirrored)
            DrawKingHalf(g, innerFrame, color, suit, true, outlinePen, thinPen, skinTone, beardColor, gold, darkGold);

            // Center dividing area with suit symbol
            float centerY = innerFrame.Y + innerFrame.Height / 2;
            using var suitFont = new Font("Segoe UI Symbol", innerFrame.Width * 0.12f, FontStyle.Bold);
            using var suitBrush = new SolidBrush(color);
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            RectangleF suitRect = new RectangleF(innerFrame.X, centerY - innerFrame.Width * 0.1f, 
                                                  innerFrame.Width, innerFrame.Width * 0.2f);

            // White background for center suit
            using var whiteBrush = new SolidBrush(Color.White);
            g.FillEllipse(whiteBrush, innerFrame.X + innerFrame.Width * 0.35f, centerY - innerFrame.Width * 0.15f,
                          innerFrame.Width * 0.3f, innerFrame.Width * 0.3f);
            g.DrawString(suit, suitFont, suitBrush, suitRect, sf);
        }

        private static void DrawKingHalf(Graphics g, RectangleF rect, Color color, string suit, bool mirrored,
            Pen outlinePen, Pen thinPen, Color skinTone, Color beardColor, Color gold, Color darkGold)
        {
            var state = g.Save();

            if (mirrored)
            {
                g.TranslateTransform(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                g.RotateTransform(180);
                g.TranslateTransform(-(rect.X + rect.Width / 2), -(rect.Y + rect.Height / 2));
            }

            float halfH = rect.Height * 0.48f;
            RectangleF half = new RectangleF(rect.X, rect.Y, rect.Width, halfH);

            // Royal robe
            using var robeBrush = new SolidBrush(Color.FromArgb(220, color));
            using var ermineBrush = new SolidBrush(Color.White);
            using var ermineSpotBrush = new SolidBrush(Color.FromArgb(40, 30, 20));

            // Robe shape
            PointF[] robe = {
                new PointF(half.X + half.Width * 0.12f, half.Bottom),
                new PointF(half.X + half.Width * 0.18f, half.Y + half.Height * 0.5f),
                new PointF(half.X + half.Width * 0.25f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.4f),
                new PointF(half.X + half.Width * 0.75f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.82f, half.Y + half.Height * 0.5f),
                new PointF(half.X + half.Width * 0.88f, half.Bottom)
            };
            g.FillPolygon(robeBrush, robe);
            g.DrawPolygon(outlinePen, robe);

            // Ermine collar (white with black spots - royal trim)
            PointF[] collar = {
                new PointF(half.X + half.Width * 0.28f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.35f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.55f),
                new PointF(half.X + half.Width * 0.65f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.72f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.4f)
            };
            g.FillPolygon(ermineBrush, collar);
            g.DrawPolygon(thinPen, collar);

            // Ermine spots
            float spotSize = half.Width * 0.015f;
            g.FillEllipse(ermineSpotBrush, half.X + half.Width * 0.4f, half.Y + half.Height * 0.47f, spotSize, spotSize * 2);
            g.FillEllipse(ermineSpotBrush, half.X + half.Width * 0.5f, half.Y + half.Height * 0.5f, spotSize, spotSize * 2);
            g.FillEllipse(ermineSpotBrush, half.X + half.Width * 0.6f, half.Y + half.Height * 0.47f, spotSize, spotSize * 2);

            // Gold chain/medallion
            using var goldBrush = new SolidBrush(gold);
            using var goldPen = new Pen(gold, outlinePen.Width * 1.5f);
            float medalX = half.X + half.Width * 0.5f;
            float medalY = half.Y + half.Height * 0.58f;
            float medalSize = half.Width * 0.08f;
            g.DrawArc(goldPen, half.X + half.Width * 0.35f, half.Y + half.Height * 0.48f, 
                      half.Width * 0.3f, half.Height * 0.15f, 0, 180);
            g.FillEllipse(goldBrush, medalX - medalSize/2, medalY, medalSize, medalSize);
            g.DrawEllipse(outlinePen, medalX - medalSize/2, medalY, medalSize, medalSize);

            // Head
            float headW = half.Width * 0.22f;
            float headH = half.Height * 0.3f;
            RectangleF headRect = new RectangleF(half.X + half.Width * 0.5f - headW / 2, 
                                                  half.Y + half.Height * 0.12f, headW, headH);
            using var skinBrush = new SolidBrush(skinTone);
            g.FillEllipse(skinBrush, headRect);
            g.DrawEllipse(outlinePen, headRect);

            // Beard
            using var beardBrush = new SolidBrush(beardColor);
            RectangleF beardRect = new RectangleF(headRect.X + headW * 0.15f, headRect.Y + headH * 0.5f,
                                                   headW * 0.7f, headH * 0.6f);
            g.FillEllipse(beardBrush, beardRect);

            // Mustache
            g.FillEllipse(beardBrush, headRect.X + headW * 0.1f, headRect.Y + headH * 0.45f, headW * 0.35f, headH * 0.2f);
            g.FillEllipse(beardBrush, headRect.X + headW * 0.55f, headRect.Y + headH * 0.45f, headW * 0.35f, headH * 0.2f);

            // Eyes
            float eyeSize = headW * 0.1f;
            using var eyeBrush = new SolidBrush(Color.FromArgb(60, 40, 20));
            g.FillEllipse(eyeBrush, headRect.X + headW * 0.25f, headRect.Y + headH * 0.3f, eyeSize, eyeSize * 0.6f);
            g.FillEllipse(eyeBrush, headRect.X + headW * 0.65f, headRect.Y + headH * 0.3f, eyeSize, eyeSize * 0.6f);

            // Crown (elaborate royal crown)
            float crownH = headH * 0.55f;
            float crownW = headW * 1.4f;
            float crownX = headRect.X - headW * 0.2f;
            float crownY = headRect.Y - crownH * 0.5f;

            // Crown base
            RectangleF crownBase = new RectangleF(crownX, crownY + crownH * 0.6f, crownW, crownH * 0.4f);
            g.FillRectangle(goldBrush, crownBase);
            g.DrawRectangle(outlinePen, crownBase.X, crownBase.Y, crownBase.Width, crownBase.Height);

            // Crown points
            PointF[] crownPts = {
                new PointF(crownX, crownY + crownH * 0.6f),
                new PointF(crownX + crownW * 0.1f, crownY + crownH * 0.2f),
                new PointF(crownX + crownW * 0.2f, crownY + crownH * 0.5f),
                new PointF(crownX + crownW * 0.3f, crownY),
                new PointF(crownX + crownW * 0.4f, crownY + crownH * 0.4f),
                new PointF(crownX + crownW * 0.5f, crownY - crownH * 0.1f),
                new PointF(crownX + crownW * 0.6f, crownY + crownH * 0.4f),
                new PointF(crownX + crownW * 0.7f, crownY),
                new PointF(crownX + crownW * 0.8f, crownY + crownH * 0.5f),
                new PointF(crownX + crownW * 0.9f, crownY + crownH * 0.2f),
                new PointF(crownX + crownW, crownY + crownH * 0.6f)
            };
            g.FillPolygon(goldBrush, crownPts);
            g.DrawPolygon(outlinePen, crownPts);

            // Jewels on crown
            float jewelSize = crownW * 0.07f;
            using var jewelBrush = new SolidBrush(color);
            using var rubyBrush = new SolidBrush(Color.FromArgb(220, 20, 60));
            g.FillEllipse(rubyBrush, crownX + crownW * 0.5f - jewelSize/2, crownY - crownH * 0.1f - jewelSize * 0.3f, jewelSize, jewelSize);
            g.FillEllipse(jewelBrush, crownX + crownW * 0.3f - jewelSize/2, crownY - jewelSize * 0.3f, jewelSize * 0.8f, jewelSize * 0.8f);
            g.FillEllipse(jewelBrush, crownX + crownW * 0.7f - jewelSize/2, crownY - jewelSize * 0.3f, jewelSize * 0.8f, jewelSize * 0.8f);

            // Cross on top of crown
            float crossW = crownW * 0.06f;
            float crossH = crownH * 0.25f;
            float crossX = crownX + crownW * 0.5f - crossW / 2;
            float crossY = crownY - crownH * 0.1f - crossH;
            g.FillRectangle(goldBrush, crossX, crossY, crossW, crossH);
            g.FillRectangle(goldBrush, crossX - crossW, crossY + crossH * 0.2f, crossW * 3, crossW);

            // Sword (typical King attribute - behind shoulder)
            float swordX = half.X + half.Width * 0.08f;
            using var bladeBrush = new SolidBrush(Color.Silver);
            using var handleBrush = new SolidBrush(darkGold);

            // Blade
            PointF[] blade = {
                new PointF(swordX + half.Width * 0.02f, half.Y + half.Height * 0.05f),
                new PointF(swordX + half.Width * 0.05f, half.Y + half.Height * 0.05f),
                new PointF(swordX + half.Width * 0.04f, half.Y + half.Height * 0.65f),
                new PointF(swordX + half.Width * 0.03f, half.Y + half.Height * 0.65f)
            };
            g.FillPolygon(bladeBrush, blade);
            g.DrawPolygon(thinPen, blade);

            // Guard
            g.FillRectangle(handleBrush, swordX - half.Width * 0.02f, half.Y + half.Height * 0.65f, 
                            half.Width * 0.1f, half.Width * 0.02f);

            // Handle
            g.FillRectangle(handleBrush, swordX + half.Width * 0.02f, half.Y + half.Height * 0.67f, 
                            half.Width * 0.03f, half.Height * 0.15f);

            // Pommel
            g.FillEllipse(goldBrush, swordX + half.Width * 0.015f, half.Y + half.Height * 0.8f,
                          half.Width * 0.04f, half.Width * 0.04f);

            g.Restore(state);
        }

        private GraphicsPath RoundedRectangle(Rectangle r, int d)
        {
            GraphicsPath path = new GraphicsPath();

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
            path.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            path.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }

}
