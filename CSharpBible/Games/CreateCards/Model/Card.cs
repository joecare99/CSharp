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
                PntSuits=new[]{ new PointF(0.7f, 0.25f), new PointF(0.7f, 0.35f), new PointF(0.7f, 0.45f), new PointF(0.7f, 0.55f),new PointF(0.7f, 0.65f),
                                 new PointF(0.6f, 0.75f),new PointF(0.5f, 0.75f),new PointF(0.4f, 0.75f), new PointF(0.3f, 0.65f) } } },
            { CardValues.Queen ,new("Q"){ 
                PntSuits=new[]{ new PointF(0.3f, 0.35f), new PointF(0.3f, 0.45f), new PointF(0.3f, 0.55f),new PointF(0.3f, 0.65f),
                                new PointF(0.7f, 0.35f), new PointF(0.7f, 0.45f), new PointF(0.7f, 0.55f), new PointF(0.7f, 0.75f),
                                new PointF(0.4f, 0.3f), new PointF(0.5f, 0.25f),new PointF(0.6f, 0.3f),
                                new PointF(0.4f, 0.7f), new PointF(0.5f, 0.75f),new PointF(0.6f, 0.65f),
                                new PointF(0.5f, 0.55f) } } },
            { CardValues.King ,new("K"){ 
                PntSuits=new[]{ new PointF(0.3f, 0.25f), new PointF(0.3f, 0.35f), new PointF(0.3f, 0.45f), new PointF(0.3f, 0.55f),new PointF(0.3f, 0.75f),
                                new PointF(0.3f, 0.65f), new PointF(0.4f, 0.55f), new PointF(0.5f, 0.45f), new PointF(0.6f, 0.35f), new PointF(0.7f, 0.25f),
                                new PointF(0.566667f, 0.55f), new PointF(0.63333f, 0.65f), new PointF(0.7f, 0.75f) } } },
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
                foreach (var pnt in drawDef.PntVals)
                    DrawText(rect, drawDef.PrintValue, drawDef.ValSize, pnt, g);
                foreach (var pnt in drawDef.PntSVals)
                    DrawText(rect, Suit, drawDef.ValSize, pnt, g);
                foreach (var pnt in drawDef.PntSuits)
                    DrawText(rect, Suit, drawDef.SuitSize, pnt, g);

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
