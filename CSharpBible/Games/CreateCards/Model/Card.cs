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
        // Helper methods for color manipulation (replacing ControlPaint)
        private static Color DarkenColor(Color color, float factor)
        {
            int r = (int)(color.R * (1 - factor));
            int g = (int)(color.G * (1 - factor));
            int b = (int)(color.B * (1 - factor));
            return Color.FromArgb(color.A, System.Math.Max(0, r), System.Math.Max(0, g), System.Math.Max(0, b));
        }

        private static Color LightenColor(Color color, float factor)
        {
            int r = (int)(color.R + (255 - color.R) * factor);
            int g = (int)(color.G + (255 - color.G) * factor);
            int b = (int)(color.B + (255 - color.B) * factor);
            return Color.FromArgb(color.A, System.Math.Min(255, r), System.Math.Min(255, g), System.Math.Min(255, b));
        }

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
                Color.FromArgb(255, color), Color.FromArgb(200, DarkenColor(color, 0.2f)), 0f);

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
            RectangleF capRect = new RectangleF(headRect.X - headW * 0.2f, headRect.Y - headH * 0.35f, headW * 1.6f, headH * 0.6f);
            using var capGradient = new LinearGradientBrush(capRect,
                LightenColor(color, 0.2f), color, 90f);
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

            // Eyes - two symmetric eyes (front view like Queen and King)
            float eyeW = headW * 0.12f;
            float eyeH = headH * 0.07f;
            float eyeY = headRect.Y + headH * 0.36f;

            // Left eye
            float leftEyeX = headRect.X + headW * 0.22f;
            DrawJackEye(g, leftEyeX, eyeY, eyeW, eyeH, thinPen);

            // Right eye
            float rightEyeX = headRect.X + headW * 0.66f;
            DrawJackEye(g, rightEyeX, eyeY, eyeW, eyeH, thinPen);

            // Eyebrows
            using var browPen = new Pen(hairColor, thinPen.Width * 1.2f);
            g.DrawArc(browPen, leftEyeX - eyeW * 0.1f, eyeY - eyeH * 1.0f, eyeW * 1.2f, eyeH, 200, 70);
            g.DrawArc(browPen, rightEyeX - eyeW * 0.1f, eyeY - eyeH * 1.0f, eyeW * 1.2f, eyeH, 250, 70);

            // Nose (centered)
            g.DrawBezier(thinPen,
                new PointF(headRect.X + headW * 0.48f, headRect.Y + headH * 0.4f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y + headH * 0.48f),
                new PointF(headRect.X + headW * 0.52f, headRect.Y + headH * 0.54f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y + headH * 0.56f));

            // Mouth (centered)
            using var lipPen = new Pen(Color.FromArgb(180, 100, 90), thinPen.Width);
            g.DrawArc(lipPen, headRect.X + headW * 0.4f, headRect.Y + headH * 0.62f, headW * 0.2f, headH * 0.1f, 0, 180);

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
                Color.FromArgb(255, color), Color.FromArgb(200, DarkenColor(color, 0.15f)), 90f);

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
            RectangleF petalRect = new RectangleF(x - size, y - size, size * 2, size * 2);
            using var petalGradient = new LinearGradientBrush(petalRect,
                LightenColor(petalColor, 0.3f), petalColor, 45f);

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

                using var innerBrush = new SolidBrush(LightenColor(petalColor, 0.2f));
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

        private static void DrawJackEye(Graphics g, float x, float y, float w, float h, Pen outlinePen)
        {
            // Eye white
            using var whiteBrush = new SolidBrush(Color.FromArgb(252, 252, 252));
            g.FillEllipse(whiteBrush, x, y, w, h);

            // Iris
            using var irisBrush = new SolidBrush(Color.FromArgb(80, 60, 40));
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
        }

        private static void DrawKing(Graphics g, Rectangle rect, Color color, string suit)
        {
            // French-style King (Roi) - elaborate mirrored double-headed design
            float penW = System.Math.Max(1f, rect.Width / 120f);
            using var outlinePen = new Pen(Color.FromArgb(50, 30, 15), penW);
            using var thinPen = new Pen(Color.FromArgb(50, 30, 15), penW * 0.5f);

            // Colors - royal palette
            Color cream = Color.FromArgb(255, 252, 245);
            Color gold = Color.FromArgb(212, 175, 55);
            Color darkGold = Color.FromArgb(170, 135, 30);
            Color lightGold = Color.FromArgb(255, 223, 120);
            Color skinTone = Color.FromArgb(255, 218, 185);
            Color skinShadow = Color.FromArgb(235, 190, 160);
            Color beardColor = Color.FromArgb(110, 75, 40);
            Color beardHighlight = Color.FromArgb(150, 110, 65);

            // Draw decorative frame with gradient
            RectangleF frameRect = new RectangleF(rect.X + rect.Width * 0.10f, rect.Y + rect.Height * 0.06f,
                                                   rect.Width * 0.80f, rect.Height * 0.88f);
            using var frameBrush = new LinearGradientBrush(frameRect, cream, Color.FromArgb(250, 245, 235), 45f);
            g.FillRectangle(frameBrush, frameRect);

            // Royal cross-hatch pattern in background
            using var patternPen = new Pen(Color.FromArgb(10, color), penW * 0.3f);
            for (float i = frameRect.X; i < frameRect.Right + frameRect.Height; i += rect.Width * 0.035f)
            {
                g.DrawLine(patternPen, i, frameRect.Y, i - frameRect.Height * 0.4f, frameRect.Bottom);
                g.DrawLine(patternPen, i - frameRect.Height * 0.4f, frameRect.Y, i, frameRect.Bottom);
            }

            // Inner decorative border - triple line royal style
            float borderInset = rect.Width * 0.02f;
            RectangleF innerFrame = RectangleF.Inflate(frameRect, -borderInset * 2, -borderInset * 2);
            using var borderPen = new Pen(color, penW * 1.2f);
            using var goldBorderPen = new Pen(gold, penW * 0.6f);
            g.DrawRectangle(borderPen, innerFrame.X, innerFrame.Y, innerFrame.Width, innerFrame.Height);
            g.DrawRectangle(goldBorderPen, innerFrame.X - penW, innerFrame.Y - penW, innerFrame.Width + penW * 2, innerFrame.Height + penW * 2);
            g.DrawRectangle(goldBorderPen, innerFrame.X + penW * 2, innerFrame.Y + penW * 2, innerFrame.Width - penW * 4, innerFrame.Height - penW * 4);

            // Royal crown corner ornaments
            DrawRoyalCorner(g, innerFrame.X, innerFrame.Y, rect.Width * 0.1f, gold, color, outlinePen, false, false);
            DrawRoyalCorner(g, innerFrame.Right, innerFrame.Y, rect.Width * 0.1f, gold, color, outlinePen, true, false);
            DrawRoyalCorner(g, innerFrame.X, innerFrame.Bottom, rect.Width * 0.1f, gold, color, outlinePen, false, true);
            DrawRoyalCorner(g, innerFrame.Right, innerFrame.Bottom, rect.Width * 0.1f, gold, color, outlinePen, true, true);

            // Draw upper half figure
            DrawKingHalf(g, innerFrame, color, suit, false, outlinePen, thinPen, skinTone, skinShadow, beardColor, beardHighlight, gold, darkGold, lightGold);

            // Draw lower half figure (mirrored)
            DrawKingHalf(g, innerFrame, color, suit, true, outlinePen, thinPen, skinTone, skinShadow, beardColor, beardHighlight, gold, darkGold, lightGold);

            // Center medallion with suit symbol and royal seal
            float centerY = innerFrame.Y + innerFrame.Height / 2;
            float medalSize = innerFrame.Width * 0.26f;
            RectangleF medalRect = new RectangleF(innerFrame.X + innerFrame.Width / 2 - medalSize / 2,
                                                   centerY - medalSize / 2, medalSize, medalSize);

            // Royal seal medallion with multiple rings
            using var sealGradient = new LinearGradientBrush(medalRect, Color.White, Color.FromArgb(245, 240, 225), 135f);
            g.FillEllipse(sealGradient, medalRect);

            // Decorative rings
            using var outerRingPen = new Pen(gold, penW * 2.5f);
            g.DrawEllipse(outerRingPen, medalRect);
            using var innerRingPen = new Pen(color, penW * 1.2f);
            g.DrawEllipse(innerRingPen, RectangleF.Inflate(medalRect, -penW * 3, -penW * 3));
            using var centerRingPen = new Pen(gold, penW * 0.8f);
            g.DrawEllipse(centerRingPen, RectangleF.Inflate(medalRect, -penW * 5, -penW * 5));

            // Suit symbol
            using var suitFont = new Font("Segoe UI Symbol", medalSize * 0.4f, FontStyle.Bold);
            using var suitBrush = new SolidBrush(color);
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(suit, suitFont, suitBrush, medalRect, sf);
        }

        private static void DrawRoyalCorner(Graphics g, float x, float y, float size, Color gold, Color accent, Pen outlinePen, bool flipH, bool flipV)
        {
            var state = g.Save();
            g.TranslateTransform(x, y);
            if (flipH) g.ScaleTransform(-1, 1);
            if (flipV) g.ScaleTransform(1, -1);

            using var goldBrush = new SolidBrush(gold);
            using var accentBrush = new SolidBrush(accent);

            // Crown-like corner ornament
            PointF[] crownOrn = {
                new PointF(0, 0),
                new PointF(size * 0.4f, 0),
                new PointF(size * 0.3f, size * 0.15f),
                new PointF(size * 0.5f, size * 0.08f),
                new PointF(size * 0.35f, size * 0.25f),
                new PointF(size * 0.25f, size * 0.35f),
                new PointF(size * 0.08f, size * 0.5f),
                new PointF(size * 0.15f, size * 0.3f),
                new PointF(0, size * 0.4f)
            };
            g.FillPolygon(goldBrush, crownOrn);
            g.DrawPolygon(outlinePen, crownOrn);

            // Small jewel
            float jewelX = size * 0.12f;
            float jewelY = size * 0.12f;
            float jewelSize = size * 0.1f;
            g.FillEllipse(accentBrush, jewelX, jewelY, jewelSize, jewelSize);

            g.Restore(state);
        }

        private static void DrawKingHalf(Graphics g, RectangleF rect, Color color, string suit, bool mirrored,
            Pen outlinePen, Pen thinPen, Color skinTone, Color skinShadow, Color beardColor, Color beardHighlight, 
            Color gold, Color darkGold, Color lightGold)
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

            // Royal robe with rich gradient and ermine trim
            RectangleF robeRect = new RectangleF(half.X + half.Width * 0.1f, half.Y + half.Height * 0.38f,
                                                  half.Width * 0.8f, half.Height * 0.62f);
            using var robeGradient = new LinearGradientBrush(robeRect,
                Color.FromArgb(255, color), Color.FromArgb(200, DarkenColor(color, 0.2f)), 90f);

            PointF[] robe = {
                new PointF(half.X + half.Width * 0.1f, half.Bottom),
                new PointF(half.X + half.Width * 0.15f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.22f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.38f),
                new PointF(half.X + half.Width * 0.78f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.85f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.9f, half.Bottom)
            };
            g.FillPolygon(robeGradient, robe);
            g.DrawPolygon(outlinePen, robe);

            // Robe folds
            using var foldPen = new Pen(Color.FromArgb(50, 0, 0, 0), thinPen.Width);
            using var highlightPen = new Pen(Color.FromArgb(60, 255, 255, 255), thinPen.Width);
            g.DrawBezier(foldPen,
                new PointF(half.X + half.Width * 0.35f, half.Y + half.Height * 0.48f),
                new PointF(half.X + half.Width * 0.32f, half.Y + half.Height * 0.65f),
                new PointF(half.X + half.Width * 0.28f, half.Y + half.Height * 0.8f),
                new PointF(half.X + half.Width * 0.22f, half.Bottom));
            g.DrawBezier(highlightPen,
                new PointF(half.X + half.Width * 0.38f, half.Y + half.Height * 0.48f),
                new PointF(half.X + half.Width * 0.36f, half.Y + half.Height * 0.65f),
                new PointF(half.X + half.Width * 0.33f, half.Y + half.Height * 0.8f),
                new PointF(half.X + half.Width * 0.28f, half.Bottom));
            g.DrawBezier(foldPen,
                new PointF(half.X + half.Width * 0.65f, half.Y + half.Height * 0.48f),
                new PointF(half.X + half.Width * 0.68f, half.Y + half.Height * 0.65f),
                new PointF(half.X + half.Width * 0.72f, half.Y + half.Height * 0.8f),
                new PointF(half.X + half.Width * 0.78f, half.Bottom));

            // Elaborate ermine collar with spots
            using var ermineBrush = new SolidBrush(Color.FromArgb(255, 252, 248));
            PointF[] collar = {
                new PointF(half.X + half.Width * 0.25f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.32f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.42f, half.Y + half.Height * 0.56f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.58f),
                new PointF(half.X + half.Width * 0.58f, half.Y + half.Height * 0.56f),
                new PointF(half.X + half.Width * 0.68f, half.Y + half.Height * 0.52f),
                new PointF(half.X + half.Width * 0.75f, half.Y + half.Height * 0.45f),
                new PointF(half.X + half.Width * 0.5f, half.Y + half.Height * 0.38f)
            };
            g.FillPolygon(ermineBrush, collar);
            g.DrawPolygon(thinPen, collar);

            // Ermine spots (black tail tips)
            using var spotBrush = new SolidBrush(Color.FromArgb(35, 25, 15));
            float spotH = half.Width * 0.012f;
            float spotW = spotH * 0.6f;
            float[] spotXs = { 0.35f, 0.42f, 0.5f, 0.58f, 0.65f };
            float[] spotYs = { 0.48f, 0.52f, 0.54f, 0.52f, 0.48f };
            for (int i = 0; i < spotXs.Length; i++)
            {
                g.FillEllipse(spotBrush, half.X + half.Width * spotXs[i], half.Y + half.Height * spotYs[i], spotW, spotH * 2.5f);
            }
            // Second row
            float[] spotXs2 = { 0.38f, 0.46f, 0.54f, 0.62f };
            for (int i = 0; i < spotXs2.Length; i++)
            {
                g.FillEllipse(spotBrush, half.X + half.Width * spotXs2[i], half.Y + half.Height * 0.44f, spotW, spotH * 2.5f);
            }

            // Royal chain with medallion
            using var chainPen = new Pen(gold, outlinePen.Width * 2f);
            g.DrawArc(chainPen, half.X + half.Width * 0.35f, half.Y + half.Height * 0.48f,
                      half.Width * 0.3f, half.Height * 0.16f, 0, 180);

            // Chain links detail
            using var linkPen = new Pen(darkGold, thinPen.Width);
            for (float lx = half.X + half.Width * 0.38f; lx < half.X + half.Width * 0.62f; lx += half.Width * 0.04f)
            {
                float ly = half.Y + half.Height * 0.52f + (float)System.Math.Sin((lx - half.X) * 0.1f) * half.Height * 0.04f;
                g.DrawEllipse(linkPen, lx, ly, half.Width * 0.025f, half.Height * 0.02f);
            }

            // Royal medallion on chain
            using var medalGradient = new LinearGradientBrush(
                new RectangleF(half.X + half.Width * 0.44f, half.Y + half.Height * 0.58f, half.Width * 0.12f, half.Width * 0.12f),
                lightGold, gold, 135f);
            float medalX = half.X + half.Width * 0.5f;
            float medalY = half.Y + half.Height * 0.62f;
            float medalSize = half.Width * 0.1f;
            g.FillEllipse(medalGradient, medalX - medalSize / 2, medalY, medalSize, medalSize);
            g.DrawEllipse(outlinePen, medalX - medalSize / 2, medalY, medalSize, medalSize);

            // Cross on medallion
            using var crossPen = new Pen(color, thinPen.Width * 1.5f);
            g.DrawLine(crossPen, medalX, medalY + medalSize * 0.2f, medalX, medalY + medalSize * 0.8f);
            g.DrawLine(crossPen, medalX - medalSize * 0.3f, medalY + medalSize * 0.5f, medalX + medalSize * 0.3f, medalY + medalSize * 0.5f);

            // Head with detailed shading
            float headW = half.Width * 0.24f;
            float headH = half.Height * 0.32f;
            RectangleF headRect = new RectangleF(half.X + half.Width * 0.5f - headW / 2,
                                                  half.Y + half.Height * 0.08f, headW, headH);

            // Face with gradient
            using var faceGradient = new LinearGradientBrush(headRect, skinTone, skinShadow, 130f);
            g.FillEllipse(faceGradient, headRect);
            g.DrawEllipse(outlinePen, headRect);

            // Elaborate beard with gradient and texture
            using var beardGradient = new LinearGradientBrush(
                new RectangleF(headRect.X + headW * 0.1f, headRect.Y + headH * 0.45f, headW * 0.8f, headH * 0.7f),
                beardHighlight, beardColor, 90f);

            // Main beard shape
            PointF[] beard = {
                new PointF(headRect.X + headW * 0.15f, headRect.Y + headH * 0.5f),
                new PointF(headRect.X + headW * 0.1f, headRect.Y + headH * 0.7f),
                new PointF(headRect.X + headW * 0.2f, headRect.Y + headH * 0.9f),
                new PointF(headRect.X + headW * 0.35f, headRect.Y + headH * 1.05f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y + headH * 1.1f),
                new PointF(headRect.X + headW * 0.65f, headRect.Y + headH * 1.05f),
                new PointF(headRect.X + headW * 0.8f, headRect.Y + headH * 0.9f),
                new PointF(headRect.X + headW * 0.9f, headRect.Y + headH * 0.7f),
                new PointF(headRect.X + headW * 0.85f, headRect.Y + headH * 0.5f)
            };
            g.FillPolygon(beardGradient, beard);
            g.DrawPolygon(thinPen, beard);

            // Beard texture (wavy lines)
            using var beardTexturePen = new Pen(Color.FromArgb(60, 80, 55, 30), thinPen.Width * 0.5f);
            for (int i = 0; i < 5; i++)
            {
                float bx = headRect.X + headW * (0.25f + i * 0.12f);
                g.DrawBezier(beardTexturePen,
                    new PointF(bx, headRect.Y + headH * 0.55f),
                    new PointF(bx - headW * 0.03f, headRect.Y + headH * 0.7f),
                    new PointF(bx + headW * 0.03f, headRect.Y + headH * 0.85f),
                    new PointF(bx, headRect.Y + headH * 1.0f));
            }

            // Mustache with detail
            using var mustacheBrush = new SolidBrush(beardColor);
            PointF[] leftMustache = {
                new PointF(headRect.X + headW * 0.48f, headRect.Y + headH * 0.52f),
                new PointF(headRect.X + headW * 0.3f, headRect.Y + headH * 0.48f),
                new PointF(headRect.X + headW * 0.15f, headRect.Y + headH * 0.55f),
                new PointF(headRect.X + headW * 0.2f, headRect.Y + headH * 0.6f),
                new PointF(headRect.X + headW * 0.35f, headRect.Y + headH * 0.58f),
                new PointF(headRect.X + headW * 0.48f, headRect.Y + headH * 0.6f)
            };
            g.FillPolygon(mustacheBrush, leftMustache);

            PointF[] rightMustache = {
                new PointF(headRect.X + headW * 0.52f, headRect.Y + headH * 0.52f),
                new PointF(headRect.X + headW * 0.7f, headRect.Y + headH * 0.48f),
                new PointF(headRect.X + headW * 0.85f, headRect.Y + headH * 0.55f),
                new PointF(headRect.X + headW * 0.8f, headRect.Y + headH * 0.6f),
                new PointF(headRect.X + headW * 0.65f, headRect.Y + headH * 0.58f),
                new PointF(headRect.X + headW * 0.52f, headRect.Y + headH * 0.6f)
            };
            g.FillPolygon(mustacheBrush, rightMustache);

            // Detailed eyes
            float eyeW = headW * 0.13f;
            float eyeH = headH * 0.07f;
            DrawKingEye(g, headRect.X + headW * 0.22f, headRect.Y + headH * 0.32f, eyeW, eyeH, thinPen);
            DrawKingEye(g, headRect.X + headW * 0.65f, headRect.Y + headH * 0.32f, eyeW, eyeH, thinPen);

            // Stern eyebrows
            using var browPen = new Pen(beardColor, thinPen.Width * 1.5f);
            g.DrawLine(browPen, headRect.X + headW * 0.18f, headRect.Y + headH * 0.28f, 
                       headRect.X + headW * 0.38f, headRect.Y + headH * 0.25f);
            g.DrawLine(browPen, headRect.X + headW * 0.62f, headRect.Y + headH * 0.25f, 
                       headRect.X + headW * 0.82f, headRect.Y + headH * 0.28f);

            // Nose
            g.DrawBezier(thinPen,
                new PointF(headRect.X + headW * 0.48f, headRect.Y + headH * 0.35f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y + headH * 0.42f),
                new PointF(headRect.X + headW * 0.52f, headRect.Y + headH * 0.48f),
                new PointF(headRect.X + headW * 0.5f, headRect.Y + headH * 0.5f));

            // Elaborate royal crown with cross
            float crownH = headH * 0.6f;
            float crownW = headW * 1.5f;
            float crownX = headRect.X - headW * 0.25f;
            float crownY = headRect.Y - crownH * 0.45f;

            // Crown base band
            using var crownBaseGradient = new LinearGradientBrush(
                new RectangleF(crownX, crownY + crownH * 0.55f, crownW, crownH * 0.45f),
                lightGold, gold, 90f);
            RectangleF crownBase = new RectangleF(crownX, crownY + crownH * 0.55f, crownW, crownH * 0.45f);
            g.FillRectangle(crownBaseGradient, crownBase);
            g.DrawRectangle(outlinePen, crownBase.X, crownBase.Y, crownBase.Width, crownBase.Height);

            // Velvet interior (visible above base)
            using var velvetBrush = new SolidBrush(color);
            RectangleF velvetRect = new RectangleF(crownX + crownW * 0.08f, crownY + crownH * 0.4f, 
                                                    crownW * 0.84f, crownH * 0.2f);
            g.FillRectangle(velvetBrush, velvetRect);

            // Crown arches with gradient
            using var archGradient = new LinearGradientBrush(
                new RectangleF(crownX, crownY, crownW, crownH * 0.6f), lightGold, gold, 90f);

            PointF[] crownArch = {
                new PointF(crownX + crownW * 0.08f, crownY + crownH * 0.55f),
                new PointF(crownX + crownW * 0.15f, crownY + crownH * 0.2f),
                new PointF(crownX + crownW * 0.3f, crownY + crownH * 0.35f),
                new PointF(crownX + crownW * 0.5f, crownY - crownH * 0.05f),
                new PointF(crownX + crownW * 0.7f, crownY + crownH * 0.35f),
                new PointF(crownX + crownW * 0.85f, crownY + crownH * 0.2f),
                new PointF(crownX + crownW * 0.92f, crownY + crownH * 0.55f)
            };
            g.FillPolygon(archGradient, crownArch);
            g.DrawPolygon(outlinePen, crownArch);

            // Fleur-de-lis points on crown
            DrawFleurDeLis(g, crownX + crownW * 0.15f, crownY + crownH * 0.15f, crownW * 0.08f, gold, outlinePen);
            DrawFleurDeLis(g, crownX + crownW * 0.5f, crownY - crownH * 0.1f, crownW * 0.1f, gold, outlinePen);
            DrawFleurDeLis(g, crownX + crownW * 0.85f, crownY + crownH * 0.15f, crownW * 0.08f, gold, outlinePen);

            // Cross on top (monde)
            float crossX = crownX + crownW * 0.5f;
            float crossY = crownY - crownH * 0.15f;
            float crossSize = crownW * 0.08f;

            // Orb
            using var orbGradient = new LinearGradientBrush(
                new RectangleF(crossX - crossSize * 0.7f, crossY - crossSize * 0.3f, crossSize * 1.4f, crossSize * 1.4f),
                lightGold, gold, 135f);
            g.FillEllipse(orbGradient, crossX - crossSize * 0.5f, crossY, crossSize, crossSize);
            g.DrawEllipse(outlinePen, crossX - crossSize * 0.5f, crossY, crossSize, crossSize);

            // Cross on orb
            using var crossBrush = new SolidBrush(gold);
            g.FillRectangle(crossBrush, crossX - crossSize * 0.08f, crossY - crossSize * 0.5f, crossSize * 0.16f, crossSize * 0.6f);
            g.FillRectangle(crossBrush, crossX - crossSize * 0.25f, crossY - crossSize * 0.3f, crossSize * 0.5f, crossSize * 0.12f);

            // Crown jewels
            float jewelSize = crownW * 0.055f;
            using var rubyBrush = new SolidBrush(Color.FromArgb(180, 20, 40));
            using var sapphireBrush = new SolidBrush(Color.FromArgb(30, 60, 150));
            using var emeraldBrush = new SolidBrush(Color.FromArgb(30, 120, 60));

            // Band jewels
            g.FillEllipse(rubyBrush, crownX + crownW * 0.2f, crownY + crownH * 0.65f, jewelSize, jewelSize);
            g.FillEllipse(sapphireBrush, crownX + crownW * 0.4f, crownY + crownH * 0.65f, jewelSize, jewelSize);
            g.FillEllipse(emeraldBrush, crownX + crownW * 0.5f - jewelSize / 2, crownY + crownH * 0.62f, jewelSize * 1.2f, jewelSize * 1.2f);
            g.FillEllipse(sapphireBrush, crownX + crownW * 0.6f, crownY + crownH * 0.65f, jewelSize, jewelSize);
            g.FillEllipse(rubyBrush, crownX + crownW * 0.8f - jewelSize, crownY + crownH * 0.65f, jewelSize, jewelSize);

            // Jewel highlights
            using var jewelHighlight = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
            g.FillEllipse(jewelHighlight, crownX + crownW * 0.5f - jewelSize * 0.3f, crownY + crownH * 0.64f, jewelSize * 0.3f, jewelSize * 0.3f);

            // Royal sword (behind shoulder)
            DrawRoyalSword(g, half.X + half.Width * 0.06f, half.Y + half.Height * 0.02f, half.Width * 0.12f, half.Height * 0.95f, 
                          gold, darkGold, outlinePen, thinPen);

            g.Restore(state);
        }

        private static void DrawKingEye(Graphics g, float x, float y, float w, float h, Pen thinPen)
        {
            using var whiteBrush = new SolidBrush(Color.FromArgb(250, 250, 250));
            g.FillEllipse(whiteBrush, x, y, w, h);

            using var irisBrush = new SolidBrush(Color.FromArgb(70, 55, 45));
            g.FillEllipse(irisBrush, x + w * 0.28f, y + h * 0.1f, w * 0.45f, h * 0.8f);

            using var pupilBrush = new SolidBrush(Color.Black);
            g.FillEllipse(pupilBrush, x + w * 0.38f, y + h * 0.22f, w * 0.25f, h * 0.56f);

            using var highlightBrush = new SolidBrush(Color.White);
            g.FillEllipse(highlightBrush, x + w * 0.35f, y + h * 0.18f, w * 0.12f, h * 0.25f);

            g.DrawEllipse(thinPen, x, y, w, h);
        }

        private static void DrawFleurDeLis(Graphics g, float x, float y, float size, Color gold, Pen outlinePen)
        {
            using var goldBrush = new SolidBrush(gold);

            // Center petal
            PointF[] center = {
                new PointF(x, y + size),
                new PointF(x - size * 0.15f, y + size * 0.3f),
                new PointF(x, y - size * 0.2f),
                new PointF(x + size * 0.15f, y + size * 0.3f)
            };
            g.FillPolygon(goldBrush, center);

            // Left petal
            PointF[] left = {
                new PointF(x - size * 0.1f, y + size * 0.5f),
                new PointF(x - size * 0.4f, y),
                new PointF(x - size * 0.35f, y + size * 0.6f)
            };
            g.FillPolygon(goldBrush, left);

            // Right petal
            PointF[] right = {
                new PointF(x + size * 0.1f, y + size * 0.5f),
                new PointF(x + size * 0.4f, y),
                new PointF(x + size * 0.35f, y + size * 0.6f)
            };
            g.FillPolygon(goldBrush, right);
        }

        private static void DrawRoyalSword(Graphics g, float x, float y, float w, float h, Color gold, Color darkGold, Pen outlinePen, Pen thinPen)
        {
            // Blade with gradient
            using var bladeGradient = new LinearGradientBrush(
                new RectangleF(x, y, w, h * 0.7f),
                Color.FromArgb(230, 235, 240), Color.FromArgb(180, 185, 195), 0f);

            PointF[] blade = {
                new PointF(x + w * 0.35f, y),
                new PointF(x + w * 0.65f, y),
                new PointF(x + w * 0.6f, y + h * 0.65f),
                new PointF(x + w * 0.4f, y + h * 0.65f)
            };
            g.FillPolygon(bladeGradient, blade);
            g.DrawPolygon(outlinePen, blade);

            // Blade center line (fuller)
            using var fullerPen = new Pen(Color.FromArgb(60, 100, 100, 100), thinPen.Width);
            g.DrawLine(fullerPen, x + w * 0.5f, y + h * 0.05f, x + w * 0.5f, y + h * 0.55f);

            // Elaborate crossguard
            using var guardGradient = new LinearGradientBrush(
                new RectangleF(x - w * 0.3f, y + h * 0.63f, w * 1.6f, h * 0.06f),
                Color.FromArgb(255, 223, 120), gold, 90f);
            RectangleF guard = new RectangleF(x - w * 0.2f, y + h * 0.64f, w * 1.4f, h * 0.04f);
            g.FillRectangle(guardGradient, guard);
            g.DrawRectangle(outlinePen, guard.X, guard.Y, guard.Width, guard.Height);

            // Guard ends (quillons)
            using var goldBrush = new SolidBrush(gold);
            g.FillEllipse(goldBrush, x - w * 0.3f, y + h * 0.635f, w * 0.15f, h * 0.05f);
            g.FillEllipse(goldBrush, x + w * 1.15f, y + h * 0.635f, w * 0.15f, h * 0.05f);

            // Handle (grip) with wire wrapping
            using var gripGradient = new LinearGradientBrush(
                new RectangleF(x + w * 0.35f, y + h * 0.68f, w * 0.3f, h * 0.2f),
                darkGold, Color.FromArgb(120, 85, 30), 0f);
            RectangleF grip = new RectangleF(x + w * 0.38f, y + h * 0.68f, w * 0.24f, h * 0.18f);
            g.FillRectangle(gripGradient, grip);
            g.DrawRectangle(outlinePen, grip.X, grip.Y, grip.Width, grip.Height);

            // Wire wrapping
            using var wirePen = new Pen(gold, thinPen.Width * 0.7f);
            for (float wy = grip.Y + h * 0.02f; wy < grip.Bottom - h * 0.01f; wy += h * 0.025f)
            {
                g.DrawLine(wirePen, grip.X, wy, grip.Right, wy + h * 0.01f);
            }

            // Pommel
            using var pommelGradient = new LinearGradientBrush(
                new RectangleF(x + w * 0.3f, y + h * 0.85f, w * 0.4f, w * 0.4f),
                Color.FromArgb(255, 223, 120), gold, 135f);
            g.FillEllipse(pommelGradient, x + w * 0.32f, y + h * 0.86f, w * 0.36f, w * 0.36f);
            g.DrawEllipse(outlinePen, x + w * 0.32f, y + h * 0.86f, w * 0.36f, w * 0.36f);

            // Jewel in pommel
            using var jewelBrush = new SolidBrush(Color.FromArgb(150, 20, 40));
            float jewelSize = w * 0.12f;
            g.FillEllipse(jewelBrush, x + w * 0.5f - jewelSize / 2, y + h * 0.88f, jewelSize, jewelSize);
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
