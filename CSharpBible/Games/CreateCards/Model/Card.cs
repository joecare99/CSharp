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
                CustomDraw = DrawJack
            }},
            { CardValues.Queen ,new("Q"){ 
                CustomDraw =  DrawQueen
            }},
            { CardValues.King ,new("K"){ 
                CustomDraw = DrawKing
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
                    drawDef.CustomDraw(g, rect, Color);
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

        private static void DrawJack(Graphics g, Rectangle rect, Color color)
        {
            // Simple stylized Jack vector drawing
            RectangleF inner = RectangleF.Inflate(rect, -rect.Width * 0.15f, -rect.Height * 0.2f);
            using var bodyBrush = new SolidBrush(Color.FromArgb(220, color));
            using var outlinePen = new Pen(Color.Black, rect.Width / 150f);

            // Body
            RectangleF body = new RectangleF(inner.X + inner.Width * 0.25f, inner.Y + inner.Height * 0.35f,
                                             inner.Width * 0.5f, inner.Height * 0.5f);
            g.FillRectangle(bodyBrush, body);
            g.DrawRectangle(outlinePen, body.X, body.Y, body.Width, body.Height);

            // Head
            float headRadius = inner.Width * 0.18f;
            PointF headCenter = new PointF(inner.X + inner.Width * 0.5f, inner.Y + inner.Height * 0.22f);
            RectangleF head = new RectangleF(headCenter.X - headRadius, headCenter.Y - headRadius,
                                              headRadius * 2, headRadius * 2);
            using var headBrush = new SolidBrush(Color.Beige);
            g.FillEllipse(headBrush, head);
            g.DrawEllipse(outlinePen, head);

            // Cap
            RectangleF cap = new RectangleF(head.X - headRadius * 0.3f, head.Y - headRadius * 0.6f,
                                             head.Width * 1.3f, head.Height * 0.6f);
            using var capBrush = new SolidBrush(color);
            g.FillRectangle(capBrush, cap);
            g.DrawRectangle(outlinePen, cap.X, cap.Y, cap.Width, cap.Height);
        }

        private static void DrawQueen(Graphics g, Rectangle rect, Color color)
        {
            // Simple stylized Queen vector drawing
            RectangleF inner = RectangleF.Inflate(rect, -rect.Width * 0.15f, -rect.Height * 0.2f);
            using var bodyBrush = new SolidBrush(Color.FromArgb(200, color));
            using var outlinePen = new Pen(Color.Black, rect.Width / 150f);

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

            // Head
            float headRadius = inner.Width * 0.17f;
            PointF headCenter = new PointF(inner.X + inner.Width * 0.5f, inner.Y + inner.Height * 0.18f);
            RectangleF head = new RectangleF(headCenter.X - headRadius, headCenter.Y - headRadius,
                                              headRadius * 2, headRadius * 2);
            using var headBrush = new SolidBrush(Color.Beige);
            g.FillEllipse(headBrush, head);
            g.DrawEllipse(outlinePen, head);

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

        private static void DrawKing(Graphics g, Rectangle rect, Color color)
        {
            // Simple stylized King vector drawing
            RectangleF inner = RectangleF.Inflate(rect, -rect.Width * 0.15f, -rect.Height * 0.2f);
            using var robeBrush = new SolidBrush(Color.FromArgb(230, color));
            using var outlinePen = new Pen(Color.Black, rect.Width / 150f);

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

            // Head
            float headRadius = inner.Width * 0.18f;
            PointF headCenter = new PointF(inner.X + inner.Width * 0.5f, inner.Y + inner.Height * 0.24f);
            RectangleF head = new RectangleF(headCenter.X - headRadius, headCenter.Y - headRadius,
                                              headRadius * 2, headRadius * 2);
            using var headBrush = new SolidBrush(Color.Beige);
            g.FillEllipse(headBrush, head);
            g.DrawEllipse(outlinePen, head);

            // Beard
            RectangleF beard = new RectangleF(head.X + head.Width * 0.2f, headCenter.Y,
                                              head.Width * 0.6f, headRadius * 0.9f);
            using var beardBrush = new SolidBrush(Color.FromArgb(160, 120, 80));
            g.FillEllipse(beardBrush, beard);
            g.DrawEllipse(outlinePen, beard);

            // Crown (more geometric)
            float crownHeight = headRadius * 0.9f;
            PointF k1 = new PointF(head.X, head.Y);
            PointF k2 = new PointF(head.Right, head.Y);
            PointF k3 = new PointF(head.X + head.Width * 0.15f, head.Y - crownHeight);
            PointF k4 = new PointF(head.X + head.Width * 0.35f, head.Y - crownHeight * 1.2f);
            PointF k5 = new PointF(head.X + head.Width * 0.65f, head.Y - crownHeight * 1.2f);
            PointF k6 = new PointF(head.X + head.Width * 0.85f, head.Y - crownHeight);
            PointF[] crown = { k1, k3, k4, k5, k6, k2 };
            using var crownBrush = new SolidBrush(color);
            g.FillPolygon(crownBrush, crown);
            g.DrawPolygon(outlinePen, crown);
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
