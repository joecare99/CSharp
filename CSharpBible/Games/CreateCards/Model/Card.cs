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
            // Simple stylized Jack vector drawing
            RectangleF inner = RectangleF.Inflate(rect, -rect.Width * 0.15f, -rect.Height * 0.2f);
            using var bodyBrush = new SolidBrush(Color.FromArgb(220, color));
            using var outlinePen = new Pen(Color.Black, rect.Width / 150f);
            using var faceBrush = new SolidBrush(Color.Beige);

            // Body
            RectangleF body = new RectangleF(inner.X + inner.Width * 0.25f, inner.Y + inner.Height * 0.35f,
                                             inner.Width * 0.5f, inner.Height * 0.5f);
            g.FillRectangle(bodyBrush, body);
            g.DrawRectangle(outlinePen, body.X, body.Y, body.Width, body.Height);

            // Sword (left side)
            float swordW = inner.Width * 0.06f;
            float swordH = inner.Height * 0.7f;
            float swordX = inner.X + inner.Width * 0.15f;
            float swordY = inner.Bottom - swordH * 0.8f;
            
            using var metalBrush = new SolidBrush(Color.Silver);
            // Blade
            g.FillRectangle(metalBrush, swordX, swordY, swordW, swordH);
            g.DrawRectangle(outlinePen, swordX, swordY, swordW, swordH);
            // Guard
            g.FillRectangle(metalBrush, swordX - swordW * 1.5f, swordY + swordH * 0.7f, swordW * 4, swordW);
            g.DrawRectangle(outlinePen, swordX - swordW * 1.5f, swordY + swordH * 0.7f, swordW * 4, swordW);

            // Head
            float headRadius = inner.Width * 0.18f;
            PointF headCenter = new PointF(inner.X + inner.Width * 0.5f, inner.Y + inner.Height * 0.22f);
            RectangleF head = new RectangleF(headCenter.X - headRadius, headCenter.Y - headRadius,
                                              headRadius * 2, headRadius * 2);
            g.FillEllipse(faceBrush, head);
            g.DrawEllipse(outlinePen, head);

            // Eyes
            float eyeSize = headRadius * 0.2f;
            using var eyeBrush = new SolidBrush(Color.Black);
            g.FillEllipse(eyeBrush, headCenter.X - headRadius * 0.4f, headCenter.Y - headRadius * 0.2f, eyeSize, eyeSize);
            g.FillEllipse(eyeBrush, headCenter.X + headRadius * 0.4f - eyeSize, headCenter.Y - headRadius * 0.2f, eyeSize, eyeSize);

            // Cap
            RectangleF cap = new RectangleF(head.X - headRadius * 0.3f, head.Y - headRadius * 0.6f,
                                             head.Width * 1.3f, head.Height * 0.6f);
            using var capBrush = new SolidBrush(color);
            g.FillRectangle(capBrush, cap);
            g.DrawRectangle(outlinePen, cap.X, cap.Y, cap.Width, cap.Height);

            // Feather
            PointF f1 = new PointF(cap.Right, cap.Bottom);
            PointF f2 = new PointF(cap.Right + cap.Width * 0.3f, cap.Top - cap.Height * 0.5f);
            PointF f3 = new PointF(cap.Right - cap.Width * 0.1f, cap.Top);
            g.FillPolygon(capBrush, new[] { f1, f2, f3 });
            g.DrawPolygon(outlinePen, new[] { f1, f2, f3 });

            // Shield (right side)
            float shieldW = inner.Width * 0.35f;
            float shieldH = inner.Height * 0.4f;
            float shieldX = inner.Right - shieldW * 0.9f;
            float shieldY = inner.Bottom - shieldH;
            
            PointF s1 = new PointF(shieldX, shieldY);
            PointF s2 = new PointF(shieldX + shieldW, shieldY);
            PointF s3 = new PointF(shieldX + shieldW, shieldY + shieldH * 0.6f);
            PointF s4 = new PointF(shieldX + shieldW * 0.5f, shieldY + shieldH);
            PointF s5 = new PointF(shieldX, shieldY + shieldH * 0.6f);
            PointF[] shieldPts = { s1, s2, s3, s4, s5 };

            using var shieldBrush = new SolidBrush(Color.WhiteSmoke);
            g.FillPolygon(shieldBrush, shieldPts);
            g.DrawPolygon(outlinePen, shieldPts);

            // Suit on Shield
            using var suitFont = new Font("Arial", shieldW * 0.6f, FontStyle.Bold);
            using var suitBrush = new SolidBrush(color);
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            RectangleF suitRect = new RectangleF(shieldX, shieldY, shieldW, shieldH * 0.8f);
            g.DrawString(suit, suitFont, suitBrush, suitRect, sf);
        }

        private static void DrawQueen(Graphics g, Rectangle rect, Color color, string suit)
        {
            // Simple stylized Queen vector drawing
            RectangleF inner = RectangleF.Inflate(rect, -rect.Width * 0.15f, -rect.Height * 0.2f);
            using var bodyBrush = new SolidBrush(Color.FromArgb(200, color));
            using var outlinePen = new Pen(Color.Black, rect.Width / 150f);
            using var faceBrush = new SolidBrush(Color.Beige);

            // Dress (triangle)
            PointF p1 = new PointF(inner.X + inner.Width * 0.5f, inner.Bottom);
            PointF p2 = new PointF(inner.X + inner.Width * 0.2f, inner.Y + inner.Height * 0.45f);
            PointF p3 = new PointF(inner.X + inner.Width * 0.8f, inner.Y + inner.Height * 0.45f);
            PointF[] dress = { p1, p2, p3 };
            g.FillPolygon(bodyBrush, dress);
            g.DrawPolygon(outlinePen, dress);

            // Upper body
            RectangleF body = new RectangleF(inner.X + inner.Width * 0.35f, inner.Y + inner.Height * 0.32f,
                                             inner.Width * 0.3f, inner.Height * 0.2f);
            g.FillRectangle(bodyBrush, body);
            g.DrawRectangle(outlinePen, body.X, body.Y, body.Width, body.Height);

            // Hair
            float headRadius = inner.Width * 0.17f;
            PointF headCenter = new PointF(inner.X + inner.Width * 0.5f, inner.Y + inner.Height * 0.18f);
            RectangleF hairRect = new RectangleF(headCenter.X - headRadius * 1.2f, headCenter.Y - headRadius * 1.2f,
                                                 headRadius * 2.4f, headRadius * 2.4f);
            using var hairBrush = new SolidBrush(Color.Gold);
            g.FillEllipse(hairBrush, hairRect);
            g.DrawEllipse(outlinePen, hairRect);

            // Head
            RectangleF head = new RectangleF(headCenter.X - headRadius, headCenter.Y - headRadius,
                                              headRadius * 2, headRadius * 2);
            g.FillEllipse(faceBrush, head);
            g.DrawEllipse(outlinePen, head);

            // Eyes
            float eyeSize = headRadius * 0.2f;
            using var eyeBrush = new SolidBrush(Color.Black);
            g.FillEllipse(eyeBrush, headCenter.X - headRadius * 0.35f, headCenter.Y - headRadius * 0.1f, eyeSize, eyeSize);
            g.FillEllipse(eyeBrush, headCenter.X + headRadius * 0.35f - eyeSize, headCenter.Y - headRadius * 0.1f, eyeSize, eyeSize);

            // Crown
            float crownHeight = headRadius * 0.8f;
            PointF c1 = new PointF(head.X, head.Y);
            PointF c2 = new PointF(head.Right, head.Y);
            PointF c3 = new PointF(head.X + head.Width * 0.2f, head.Y - crownHeight);
            PointF c4 = new PointF(head.X + head.Width * 0.5f, head.Y - crownHeight * 1.2f);
            PointF c5 = new PointF(head.X + head.Width * 0.8f, head.Y - crownHeight);
            PointF[] crown = { c1, c3, c4, c5, c2 };
            using var crownBrush = new SolidBrush(color);
            g.FillPolygon(crownBrush, crown);
            g.DrawPolygon(outlinePen, crown);
        }

        private static void DrawKing(Graphics g, Rectangle rect, Color color, string suit)
        {
            // Simple stylized King vector drawing
            RectangleF inner = RectangleF.Inflate(rect, -rect.Width * 0.15f, -rect.Height * 0.2f);
            using var robeBrush = new SolidBrush(Color.FromArgb(230, color));
            using var outlinePen = new Pen(Color.Black, rect.Width / 150f);
            using var faceBrush = new SolidBrush(Color.Beige);
            using var goldBrush = new SolidBrush(Color.Gold);

            // Robe (rounded rectangle)
            RectangleF robe = new RectangleF(inner.X + inner.Width * 0.25f, inner.Y + inner.Height * 0.4f,
                                             inner.Width * 0.5f, inner.Height * 0.5f);
            using (GraphicsPath robePath = new GraphicsPath())
            {
                float r = robe.Width * 0.2f;
                robePath.AddArc(robe.X, robe.Y, r, r, 180, 90);
                robePath.AddArc(robe.Right - r, robe.Y, r, r, 270, 90);
                robePath.AddArc(robe.Right - r, robe.Bottom - r, r, r, 0, 90);
                robePath.AddArc(robe.X, robe.Bottom - r, r, r, 90, 90);
                robePath.CloseFigure();
                g.FillPath(robeBrush, robePath);
                g.DrawPath(outlinePen, robePath);
            }

            // Scepter (Left side)
            float scepterX = inner.X + inner.Width * 0.15f;
            float scepterY = inner.Bottom - inner.Height * 0.7f;
            float scepterW = inner.Width * 0.05f;
            float scepterH = inner.Height * 0.6f;
            g.FillRectangle(goldBrush, scepterX, scepterY, scepterW, scepterH);
            g.DrawRectangle(outlinePen, scepterX, scepterY, scepterW, scepterH);
            // Scepter Head
            float sHeadSize = inner.Width * 0.12f;
            g.FillEllipse(goldBrush, scepterX + scepterW/2 - sHeadSize/2, scepterY - sHeadSize/2, sHeadSize, sHeadSize);
            g.DrawEllipse(outlinePen, scepterX + scepterW/2 - sHeadSize/2, scepterY - sHeadSize/2, sHeadSize, sHeadSize);

            // Orb (Reichsapfel) (Right side)
            float orbSize = inner.Width * 0.25f;
            float orbX = inner.Right - orbSize * 1.2f;
            float orbY = inner.Bottom - orbSize * 1.5f;
            RectangleF orbRect = new RectangleF(orbX, orbY, orbSize, orbSize);
            g.FillEllipse(goldBrush, orbRect);
            g.DrawEllipse(outlinePen, orbRect);
            // Cross on Orb
            float oCrossW = orbSize * 0.2f;
            float oCrossH = orbSize * 0.3f;
            float oCrossX = orbX + orbSize/2 - oCrossW/2;
            float oCrossY = orbY - oCrossH * 0.8f;
            g.FillRectangle(goldBrush, oCrossX, oCrossY, oCrossW, oCrossH); // Vertical
            g.DrawRectangle(outlinePen, oCrossX, oCrossY, oCrossW, oCrossH);
            g.FillRectangle(goldBrush, oCrossX - oCrossW, oCrossY + oCrossH * 0.3f, oCrossW * 3, oCrossW); // Horizontal
            g.DrawRectangle(outlinePen, oCrossX - oCrossW, oCrossY + oCrossH * 0.3f, oCrossW * 3, oCrossW);
            
            // Suit on Orb
            using var suitFont = new Font("Arial", orbSize * 0.6f, FontStyle.Bold);
            using var suitBrush = new SolidBrush(color);
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(suit, suitFont, suitBrush, orbRect, sf);

            // Head
            float headRadius = inner.Width * 0.18f;
            PointF headCenter = new PointF(inner.X + inner.Width * 0.5f, inner.Y + inner.Height * 0.24f);
            RectangleF head = new RectangleF(headCenter.X - headRadius, headCenter.Y - headRadius,
                                              headRadius * 2, headRadius * 2);
            g.FillEllipse(faceBrush, head);
            g.DrawEllipse(outlinePen, head);

            // Eyes
            float eyeSize = headRadius * 0.2f;
            using var eyeBrush = new SolidBrush(Color.Black);
            g.FillEllipse(eyeBrush, headCenter.X - headRadius * 0.35f, headCenter.Y - headRadius * 0.2f, eyeSize, eyeSize);
            g.FillEllipse(eyeBrush, headCenter.X + headRadius * 0.35f - eyeSize, headCenter.Y - headRadius * 0.2f, eyeSize, eyeSize);

            // Beard
            RectangleF beard = new RectangleF(head.X + head.Width * 0.225f, headCenter.Y + headRadius * 0.2f,
                                              head.Width * 0.55f, headRadius * 0.85f);
            using var beardBrush = new SolidBrush(Color.FromArgb(160, 120, 80));
            g.FillEllipse(beardBrush, beard);
            g.DrawEllipse(outlinePen, beard);
            RectangleF beard2 = new RectangleF(head.X + head.Width * 0.3f, headCenter.Y + headRadius * 0.3f,
                                  head.Width * 0.4f, headRadius * 0.3f);
            g.FillEllipse(faceBrush, beard2);
            g.DrawEllipse(outlinePen, beard2);


            // 5-pointed Golden Crown
            float crownBaseY = head.Y + headRadius * 0.5f;
            float crownH = headRadius * 1.2f;
            float crownW = head.Width * 1.1f;
            float crownX = headCenter.X - crownW / 2;
            
            PointF[] crownPts = new PointF[11]; // 5 points + 2 base points
            crownPts[0] = new PointF(crownX+ crownW * 0.1f, crownBaseY); // Base Left
            crownPts[1] = new PointF(crownX, crownBaseY - crownH * 0.6f); // Point 1
            crownPts[2] = new PointF(crownX + crownW * 0.125f, crownBaseY - crownH * 0.4f); // Point 2
            crownPts[3] = new PointF(crownX + crownW * 0.25f, crownBaseY - crownH * 0.9f); // Point 2
            crownPts[4] = new PointF(crownX + crownW * 0.375f, crownBaseY - crownH * 0.5f); // Point 2
            crownPts[5] = new PointF(crownX + crownW * 0.5f, crownBaseY - crownH); // Point 3 (Middle)
            crownPts[6] = new PointF(crownX + crownW * 0.625f, crownBaseY - crownH *0.5f); // Point 3 (Middle)
            crownPts[7] = new PointF(crownX + crownW * 0.75f, crownBaseY - crownH * 0.9f); // Point 4
            crownPts[8] = new PointF(crownX + crownW * 0.875f, crownBaseY - crownH * 0.4f); // Point 4
            crownPts[9] = new PointF(crownX + crownW, crownBaseY - crownH * 0.6f); // Point 5
            crownPts[10] = new PointF(crownX + crownW *0.9f, crownBaseY); // Base Right

            g.FillPolygon(goldBrush, crownPts);
            g.DrawPolygon(outlinePen, crownPts);

            // Gemstones on Crown
            float gemSize = crownW * 0.1f;
            float gemY = crownBaseY - gemSize * 1.5f;
            using var gemBrush = new SolidBrush(color);
            
            // Gems
            g.FillEllipse(gemBrush, crownX + crownW * 0.2f, gemY, gemSize, gemSize);
            g.FillEllipse(gemBrush, crownX + crownW * 0.5f - gemSize/2, gemY - gemSize * 0.5f, gemSize, gemSize);
            g.FillEllipse(gemBrush, crownX + crownW * 0.8f - gemSize, gemY, gemSize, gemSize);

            g.FillEllipse(gemBrush, crownX + crownW * 0.0f - gemSize / 2, crownBaseY- crownH *0.6f- gemSize * 0.5f, gemSize, gemSize);
            g.FillEllipse(gemBrush, crownX + crownW * 0.25f - gemSize / 2, crownBaseY - crownH *0.9f- gemSize * 0.5f, gemSize, gemSize);
            g.FillEllipse(gemBrush, crownX + crownW * 0.5f - gemSize / 2, crownBaseY - crownH - gemSize * 0.5f, gemSize, gemSize);
            g.FillEllipse(gemBrush, crownX + crownW * 0.75f - gemSize / 2, crownBaseY - crownH *0.9f- gemSize * 0.5f, gemSize, gemSize);
            g.FillEllipse(gemBrush, crownX + crownW * 1.0f - gemSize/2, crownBaseY - crownH *0.6f- gemSize * 0.5f, gemSize, gemSize);

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
