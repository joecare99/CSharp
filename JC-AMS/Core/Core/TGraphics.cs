// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 09-23-2022
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="TGraphics.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using JCAMS.Core.Logging;
using JCAMS.Core.Math2;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace JCAMS.Core
{
    /// <summary>
    /// Class TGraphics.
    /// </summary> 
    public static class TGraphics
    {

        /// <summary>
        /// The m fill solid rect pen
        /// </summary>
        private static Pen m_FillSolidRectPen;

        /// <summary>
        /// The m fill solid rect brush
        /// </summary>
        private static SolidBrush m_FillSolidRectBrush;

        /// <summary>
        /// The m fill solid rect sf
        /// </summary>
        private static StringFormat m_FillSolidRectSF;

        public static bool xTransportFreigabeEx { get; set; } = true;

        /// <summary>
        /// Initializes static members of the <see cref="TGraphics"/> class.
        /// </summary>
        static TGraphics()
        {
            TGraphics.m_FillSolidRectPen = new Pen(Color.Black, 1f);
            TGraphics.m_FillSolidRectBrush = new SolidBrush(Color.White);
            TGraphics.m_FillSolidRectSF = new StringFormat();
            TGraphics.m_FillSolidRectSF.Alignment = StringAlignment.Center;
            TGraphics.m_FillSolidRectSF.LineAlignment = StringAlignment.Center;
        }

        /// <summary>
        /// Draws a TrafficLight to the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rect">The bounging rect.</param>
        /// <param name="ActualState">The actual state.</param>
        /// <param name="BlinkState">if set to <c>true</c> [blink state].</param>
        public static void TrafficLight(Graphics graphics, Rectangle rect, int ActualState, bool BlinkState)
        {
            Rectangle Red;
            Rectangle Yel;
            Rectangle Gre;
            Color cYel = Color.FromArgb(255, 255, 0);
            Color cGre = Color.FromArgb(0, 255, 0);
            Color cRed = Color.FromArgb(255, 0, 0);
            Color cBgr = Color.FromArgb(64, 255, 255, 255);
            Point P;
            if (rect.Height > rect.Width)
            {
                int x = rect.X;
                int top = rect.Top;
          
                Red = new Rectangle(x, top, rect.Width, (int)Math.Ceiling((double)(rect.Height - 2) / 3.0));
                Yel = new Rectangle(rect.X, rect.Top + rect.Height / 3, rect.Width, (int)Math.Ceiling((double)(rect.Height - 2) / 3.0));
                Gre = new Rectangle(rect.X, rect.Top + rect.Height / 3 * 2, rect.Width, (int)Math.Ceiling((double)(rect.Height - 2) / 3.0));
            }
            else
            {
                double Delta = (rect.Width - 3 * rect.Height) / 2;
                int x2 = rect.X + (int)Delta;
               
                Red = new Rectangle(x2, rect.Top, rect.Height, rect.Height);
                Yel = new Rectangle(rect.X + (int)Delta + rect.Height, rect.Top, rect.Height, rect.Height);
                Gre = new Rectangle(rect.X + (int)Delta + rect.Height * 2, rect.Top, rect.Height, rect.Height);
            }
            P = new Point(Red.X + Red.Width / 2, Red.Y + Red.Height / 2);
/*            FillSolidEllipse(graphics, Red, Color.White);
            FillSolidEllipse(graphics, Yel, Color.White);
            FillSolidEllipse(graphics, Gre, Color.White);*/
            if ((ActualState & 4) != 0)
                FillSolidEllipse(graphics, Yel, cYel);
            else
                FillSolidEllipse(graphics, Yel, cBgr, cYel,1);
            if ((ActualState & 2) != 0)
                FillSolidEllipse(graphics, Gre,cGre);
            else
               FillSolidEllipse(graphics, Gre, cBgr, cGre, 1);
            if ((ActualState & 8) != 0)
                FillSolidEllipse(graphics, Red, cRed);
            else
            {
                FillSolidEllipse(graphics, Red, cBgr, cRed,1);
                return;
            }
            if (BlinkState)
            {
                Pen Flash = new Pen(Color.FromArgb(255, 0, 0), 1f);
                float DeltaFlash = (float)Red.Width / 10f;
                for (int I = 5; I <= 10; I++)
                {
                    float Radius = DeltaFlash * (float)I;
                    float Diameter = Radius * 2f;
                    Rectangle Rt = new Rectangle(P.X - (int)Radius, P.Y - (int)Radius, (int)Diameter, (int)Diameter);
                    graphics.DrawEllipse(Flash, Rt);
                }
                Flash.Dispose();
            }
        }

        /// <summary>
        /// Draw the Load, Unload and Interlock-Sign.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="R">The r.</param>
        /// <param name="ActualState">The actual state.</param>
        /// <param name="BlinkState">if set to <c>true</c> [blink state].</param>
        /// <param name="Position">The position.</param>
        public static void LoadAndUnload(Graphics gr, Rectangle R, int ActualState, bool BlinkState, int Position)
        {
            Rectangle R2;
            Rectangle R3;
            Rectangle R4;
            int em;
            if (R.Height > R.Width)
            {
                int x = R.X;
                int top = R.Top;
                _ = R.Height / 3;
                R2 = new Rectangle(x, top, R.Width, (int)Math.Ceiling((double)(R.Height - 2) / 3.0));
                R3 = new Rectangle(R.X, R.Top + R.Height / 3, R.Width, (int)Math.Ceiling((double)(R.Height - 2) / 3.0));
                R4 = new Rectangle(R.X, R.Top + R.Height / 3 * 2, R.Width, (int)Math.Ceiling((double)(R.Height - 2) / 3.0));
                em = 2 * R3.Height / 3;
            }
            else
            {
                int dY = (R.Width - 3 * R.Height) / 2;
                int x2 = R.X + dY;
                _ = R.Height;
                R2 = new Rectangle(x2, R.Top, R.Height, R.Height);
                R3 = new Rectangle(R.X + dY + R.Height, R.Top, R.Height, R.Height);
                R4 = new Rectangle(R.X + dY + R.Height * 2, R.Top, R.Height, R.Height);
                em = 2 * R3.Width / 3;
            }
            if (xTransportFreigabeEx)
            {
                Color Transportfreigabe = ((ActualState & 0x200) <= 0) ? Color.Red : Color.LimeGreen;
                DrawLabel(gr, R4, em, "■", Transportfreigabe);
            }
            Color AnfBeladen = ((ActualState & 0x10) <= 0) ? Color.WhiteSmoke : Color.Blue;
            Color AnfEntladen = ((ActualState & 0x20) <= 0) ? Color.WhiteSmoke : Color.Blue;
            switch (Position)
            {
                case 0:
                    DrawLabel(gr, R3, em, "▼", AnfBeladen);
                    DrawLabel(gr, R2, em, "▲", AnfEntladen);
                    break;
                case 1:
                    DrawLabel(gr, R3, em, "◄", AnfBeladen);
                    DrawLabel(gr, R2, em, "►", AnfEntladen);
                    break;
                case 2:
                    DrawLabel(gr, R3, em, "▲", AnfBeladen);
                    DrawLabel(gr, R2, em, "▼", AnfEntladen);
                    break;
                case 3:
                    DrawLabel(gr, R3, em, "►", AnfBeladen);
                    DrawLabel(gr, R2, em, "◄", AnfEntladen);
                    break;
            }
        }

        /// <summary>
        /// Bes the und entladen1.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="R">The r.</param>
        /// <param name="ActualState">The actual state.</param>
        /// <param name="BlinkState">if set to <c>true</c> [blink state].</param>
        /// <param name="Position">The position.</param>
        public static void BeUndEntladen1(Graphics gr, Rectangle R, int ActualState, bool BlinkState, int Position)
        {
            Rectangle R2;
            Rectangle R3;
            int em;
            if (R.Height > R.Width)
            {
                int x = R.X;
                int top = R.Top;
                _ = R.Height / 3;
                R2 = new Rectangle(x, top, R.Width, (int)Math.Ceiling((double)(R.Height - 2) / 3.0));
                R3 = new Rectangle(R.X, R.Top + R.Height / 3, R.Width, (int)Math.Ceiling((double)(R.Height - 2) / 3.0));
                Rectangle R4 = new Rectangle(R.X, R.Top + R.Height / 3 * 2, R.Width, (int)Math.Ceiling((double)(R.Height - 2) / 3.0));
                em = 2 * R3.Height / 3;
            }
            else
            {
                int dY = (R.Width - 3 * R.Height) / 2;
                int x2 = R.X + dY;
                _ = R.Height;
                R2 = new Rectangle(x2, R.Top, R.Height, R.Height);
                R3 = new Rectangle(R.X + dY + R.Height, R.Top, R.Height, R.Height);
                Rectangle R4 = new Rectangle(R.X + dY + R.Height * 2, R.Top, R.Height, R.Height);
                em = 2 * R3.Width / 3;
            }
            Color Transportfreigabe = xTransportFreigabeEx ? Color.WhiteSmoke : (((ActualState & 0x200) <= 0) ? Color.Red : Color.LimeGreen);
            Color AnfBeladen = ((ActualState & 0x10) <= 0 || !BlinkState) ? Transportfreigabe : Color.Blue;
            Color AnfEntladen = ((ActualState & 0x20) <= 0 || !BlinkState) ? Transportfreigabe : Color.Blue;
            switch (Position)
            {
                case 0:
                    DrawLabel(gr, R3, em, "▼", AnfBeladen);
                    DrawLabel(gr, R2, em, "▲", AnfEntladen);
                    break;
                case 1:
                    DrawLabel(gr, R3, em, "◄", AnfBeladen);
                    DrawLabel(gr, R2, em, "►", AnfEntladen);
                    break;
                case 2:
                    DrawLabel(gr, R3, em, "▲", AnfBeladen);
                    DrawLabel(gr, R2, em, "▼", AnfEntladen);
                    break;
                case 3:
                    DrawLabel(gr, R3, em, "►", AnfBeladen);
                    DrawLabel(gr, R2, em, "◄", AnfEntladen);
                    break;
            }
        }

        /// <summary>
        /// Draws the agv arrow head.
        /// </summary>
        /// <param name="Gr">The gr.</param>
        /// <param name="Brush">The brush.</param>
        /// <param name="Pen">The pen.</param>
        /// <param name="PtDest">The pt dest.</param>
        /// <param name="Angle">The angle.</param>
        /// <param name="Length">The length.</param>
        /// <param name="Speed">The speed.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DrawAGVArrowHead(Graphics Gr, Brush Brush, Pen Pen, PointF PtDest, double Angle, int Length, double Speed)
        {
            Point Pt2 = default(Point);
            Point Pt3 = default(Point);
            Point Pt4 = default(Point);
            Point Pt5 = default(Point);
            Point[] Pt = new Point[3]
            {
                Pt2,
                Pt3,
CMath2.ToPoint(PtDest)
            };
            double Len = Length;
            PtDest = CMath2.PointVectorToPoint(PtDest, Angle, Length * 2);
            Pt2.X = (int)((double)PtDest.X - Math.Cos((Angle + 20.0) * Math.PI / 180.0) * (double)Length * 2.0);
            Pt2.Y = (int)((double)PtDest.Y - Math.Sin((Angle + 20.0) * Math.PI / 180.0) * (double)Length * 2.0);
            Pt3.X = (int)((double)PtDest.X - Math.Cos((Angle - 20.0) * Math.PI / 180.0) * (double)Length * 2.0);
            Pt3.Y = (int)((double)PtDest.Y - Math.Sin((Angle - 20.0) * Math.PI / 180.0) * (double)Length * 2.0);
            Pt[0] = Pt2;
            Pt[1] = CMath2.ToPoint(PtDest);
            Pt[2] = Pt3;
            Gr.FillPolygon(Brush, Pt, FillMode.Alternate);
            Gr.DrawPolygon(Pens.Red, Pt);
            if (Speed > 0.0)
            {
                Pt4 = CMath2.ToPoint(CMath2.PointVectorToPoint(Pt2, Angle, Length * 2));
                Pt5 = CMath2.ToPoint(CMath2.PointVectorToPoint(Pt3, Angle, Length * 2));
            }
            else
            {
                Pt4 = CMath2.ToPoint(CMath2.PointVectorToPoint(Pt2, Angle + 180.0, Length * 2));
                Pt5 = CMath2.ToPoint(CMath2.PointVectorToPoint(Pt3, Angle + 180.0, Length * 2));
            }
            Gr.DrawLines(Pen, new Point[4]
            {
                Pt4,
                Pt2,
                Pt3,
                Pt5
            });
            return true;
        }

        /// <summary>
        /// Draws the arrow head.
        /// </summary>
        /// <param name="Gr">The gr.</param>
        /// <param name="PtDest">The pt dest.</param>
        /// <param name="Angle">The angle.</param>
        /// <param name="Brush">The brush.</param>
        /// <param name="Length">The length.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DrawArrowHead(Graphics Gr, PointF PtDest, double Angle, Brush Brush, int Length)
        {
            Point Pt2 = default(Point);
            Point Pt3 = default(Point);
            Point[] Pt = new Point[3]
            {
                Pt2,
                Pt3,
CMath2.ToPoint(PtDest)
            };
            Pt2.X = (int)((double)PtDest.X - Math.Cos((Angle + 20.0) * Math.PI / 180.0) * (double)Length);
            Pt2.Y = (int)((double)PtDest.Y - Math.Sin((Angle + 20.0) * Math.PI / 180.0) * (double)Length);
            Pt3.X = (int)((double)PtDest.X - Math.Cos((Angle - 20.0) * Math.PI / 180.0) * (double)Length);
            Pt3.Y = (int)((double)PtDest.Y - Math.Sin((Angle - 20.0) * Math.PI / 180.0) * (double)Length);
            Pt[0] = Pt2;
            Pt[1] = CMath2.ToPoint(PtDest);
            Pt[2] = Pt3;
            Gr.FillPolygon(Brush, Pt, FillMode.Alternate);
            return true;
        }

        /// <summary>
        /// Draws the arrow to.
        /// </summary>
        /// <param name="Gr">The gr.</param>
        /// <param name="Pen">The pen.</param>
        /// <param name="Center">The center.</param>
        /// <param name="Size">The size.</param>
        /// <param name="Direction">The direction.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DrawArrowTo(Graphics Gr, Pen Pen, PointF Center, int Size, float Direction)
        {
            try
            {
                Point[] Pts = new Point[4];
                int Sqrt2xSize2 = (int)Math.Sqrt(2 * Size * Size);
                Pts[0] = CMath2.ToPoint(CMath2.PointVectorToPoint(Center, Direction - 45f, Math.Sqrt(2 * Size * Size)));
                Pts[1] = CMath2.ToPoint(CMath2.PointVectorToPoint(Center, Direction - 90f, Size));
                Pts[2] = CMath2.ToPoint(CMath2.PointVectorToPoint(Center, Direction + 90f, Size));
                Pts[3] = CMath2.ToPoint(CMath2.PointVectorToPoint(Center, Direction + 45f, Math.Sqrt(2 * Size * Size)));
                Gr.DrawLines(Pen, Pts);
                return true;
            }
            catch (Exception Ex)
            {
                TLogging.Journalize(Ex);
                return false;
            }
        }

        /// <summary>
        /// Draws the bulls eye.
        /// </summary>
        /// <param name="Gr">The gr.</param>
        /// <param name="Pen">The pen.</param>
        /// <param name="Center">The center.</param>
        /// <param name="Radius">The radius.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DrawBullsEye(Graphics Gr, Pen Pen, PointF Center, int Radius)
        {
            try
            {
                int Durchmesser = 2 * Radius;
                int Radius2 = Radius / 2 * 3;
                Gr.DrawLine(Pen, Center.X, Center.Y - (float)Radius2, Center.X, Center.Y + (float)Radius2);
                Gr.DrawLine(Pen, Center.X - (float)Radius2, Center.Y, Center.X + (float)Radius2, Center.Y);
                Gr.DrawEllipse(Pen, Center.X - (float)Radius, Center.Y - (float)Radius, Durchmesser, Durchmesser);
                return true;
            }
            catch (Exception Ex)
            {
                TLogging.Journalize(Ex);
                return false;
            }
        }

        /// <summary>
        /// Draws the circle.
        /// </summary>
        /// <param name="Gr">The gr.</param>
        /// <param name="Pen">The pen.</param>
        /// <param name="Center">The center.</param>
        /// <param name="Radius">The radius.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DrawCircle(Graphics Gr, Pen Pen, PointF Center, int Radius)
        {
            try
            {
                Gr.DrawEllipse(Pen, Center.X - (float)Radius, Center.Y - (float)Radius, 2 * Radius, 2 * Radius);
                return true;
            }
            catch (Exception Ex)
            {
                TLogging.Journalize(Ex);
                return false;
            }
        }

        /// <summary>
        /// Draws the cross.
        /// </summary>
        /// <param name="Gr">The gr.</param>
        /// <param name="Pen">The pen.</param>
        /// <param name="Center">The center.</param>
        /// <param name="Size">The size.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DrawCross(Graphics Gr, Pen Pen, PointF Center, int Size)
        {
            try
            {
                int SizeDiv2 = Size / 2;
                Gr.DrawLine(Pen, Center.X, Center.Y - (float)SizeDiv2, Center.X, Center.Y + (float)SizeDiv2);
                Gr.DrawLine(Pen, Center.X - (float)SizeDiv2, Center.Y, Center.X + (float)SizeDiv2, Center.Y);
                return true;
            }
            catch (Exception Ex)
            {
                TLogging.Journalize(Ex);
                return false;
            }
        }

        /// <summary>
        /// Draws the label.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="R">The r.</param>
        /// <param name="emSize">Size of the em.</param>
        /// <param name="Text">The text.</param>
        /// <param name="rgb">The RGB.</param>
        public static void DrawLabel(Graphics gr, Rectangle R, int emSize, string Text, Color rgb)
        {
            DrawLabel(gr, R.X, R.Y, R.Width, R.Height, emSize, Text, rgb);
        }

        /// <summary>
        /// Draws the label.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="Width">The width.</param>
        /// <param name="Height">The height.</param>
        /// <param name="emSize">Size of the em.</param>
        /// <param name="Text">The text.</param>
        /// <param name="rgb">The RGB.</param>
        public static void DrawLabel(Graphics gr, int x, int y, int Width, int Height, int emSize, string Text, Color rgb)
        {
            emSize = Math.Max(emSize, 1);
            m_FillSolidRectBrush.Color = rgb;
            using (Font Font = new Font("Arial", emSize))
            {
                gr.DrawString(Text, Font, m_FillSolidRectBrush, new Rectangle(x, y, Width, Height), m_FillSolidRectSF);
                Font.Dispose();
            }
        }

        /// <summary>
        /// Draws the label.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="Width">The width.</param>
        /// <param name="Height">The height.</param>
        /// <param name="Font">The font.</param>
        /// <param name="Text">The text.</param>
        /// <param name="rgb">The RGB.</param>
        public static void DrawLabel(Graphics gr, int x, int y, int Width, int Height, Font Font, string Text, Color rgb)
        {
            try
            {
                m_FillSolidRectBrush.Color = rgb;
                gr.DrawString(Text, Font, m_FillSolidRectBrush, new Rectangle(x, y, Width, Height), m_FillSolidRectSF);
            }
            catch (Exception Ex)
            {
                TLogging.Journalize(EJournalTopic.Graphic, 10000, Core.TExceptionHelper.AsString(Ex));
            }
        }

        /// <summary>
        /// Draws the label.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="Width">The width.</param>
        /// <param name="Height">The height.</param>
        /// <param name="Text">The text.</param>
        /// <param name="rgb">The RGB.</param>
        public static void DrawLabel(Graphics gr, int x, int y, int Width, int Height, string Text, Color rgb)
        {
            Font pFont = GetFittingFont(gr, Text, "Arial", FontStyle.Regular, new Rectangle(x, y, Width, Height));
            DrawLabel(gr, x, y, Width, Height, (int)pFont.Size, Text, rgb);
        }

        /// <summary>
        /// Draws the rectangular led.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Rect">The rect.</param>
        /// <param name="BaseColor">Color of the base.</param>
        /// <param name="IsActive">if set to <c>true</c> [is active].</param>
        public static void DrawRectangularLED(Graphics gr, Rectangle Rect, Color BaseColor, bool IsActive)
        {
            int Diameter = Math.Min(Rect.Width, Rect.Height);
            var tLEDBrush = CalcLEDBrush(BaseColor, IsActive);
            Rectangle InnerRect = new Rectangle(Rect.Left + Rect.Width / 4, Rect.Top + Rect.Height / 4, Rect.Width / 4, Rect.Height / 4);
            gr.FillRectangle(tLEDBrush.bBase, Rect);
            gr.FillRectangle(tLEDBrush.bInner, InnerRect);
            Pen Pen;
            using (Pen = new Pen(tLEDBrush.bOuter, Math.Max(2, Diameter / 10)))
            {
                gr.DrawRectangle(Pen, Rect);
                Pen.Dispose();
            }
        }

        /// <summary>
        /// Draws the round led.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Rect">The rect.</param>
        /// <param name="BaseColor">Color of the base.</param>
        /// <param name="IsActive">if set to <c>true</c> [is active].</param>
        public static void DrawRoundLED(Graphics gr, Rectangle Rect, Color BaseColor, bool IsActive)
        {
            int Diameter = Math.Min(Rect.Width, Rect.Height);
            var tLEDBrush = CalcLEDBrush(BaseColor, IsActive);
            gr.FillEllipse(tLEDBrush.bBase, Rect);
            gr.FillEllipse(tLEDBrush.bInner, Rect.Left + Rect.Width / 2 - Rect.Width / 4, Rect.Top + Rect.Height / 2 - Rect.Width / 4, Rect.Width / 4, Rect.Height / 4);
            Pen Pen;
            using (Pen = new Pen(tLEDBrush.bOuter, Math.Max(2, Diameter / 10)))
            {
                gr.DrawEllipse(Pen, Rect);
                Pen.Dispose();
            }
        }

        /// <summary>
        /// Draws the triangular led.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Rect">The rect.</param>
        /// <param name="BaseColor">Color of the base.</param>
        /// <param name="IsActive">if set to <c>true</c> [is active].</param>
        public static void DrawTriangularLED(Graphics gr, Rectangle Rect, Color BaseColor, bool IsActive)
        {
            int Diameter = Math.Min(Rect.Width, Rect.Height);
            var tLEDBrush = CalcLEDBrush(BaseColor, IsActive);
            Rectangle InnerRect = new Rectangle(Rect.Left + Rect.Width / 4, Rect.Top + Rect.Height / 4, Rect.Width / 4, Rect.Height / 4);
            Point[] Points = new Point[4];
            Point[] InnerPoints = new Point[4];
            Points[0] = new Point(Rect.Left, Rect.Top);
            Points[1] = new Point(Rect.Right, Rect.Top);
            Points[2] = new Point(Rect.Left, Rect.Bottom);
            Points[3] = new Point(Rect.Left, Rect.Top);
            gr.FillPolygon(tLEDBrush.bBase, Points);
            InnerPoints[0] = new Point(InnerRect.Left, InnerRect.Top);
            InnerPoints[1] = new Point(InnerRect.Right, InnerRect.Top);
            InnerPoints[2] = new Point(InnerRect.Left, InnerRect.Bottom);
            InnerPoints[3] = new Point(InnerRect.Left, InnerRect.Top);
            gr.FillPolygon(tLEDBrush.bInner, InnerPoints);
            Pen Pen;
            using (Pen = new Pen(tLEDBrush.bOuter, Math.Max(2, Diameter / 10)))
            {
                gr.DrawPolygon(Pen, Points);
                Pen.Dispose();
            }
        }

        /// <summary>
        /// Draws the x.
        /// </summary>
        /// <param name="Gr">The gr.</param>
        /// <param name="Pen">The pen.</param>
        /// <param name="Center">The center.</param>
        /// <param name="Size">The size.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DrawX(Graphics Gr, Pen Pen, PointF Center, int Size)
        {
            try
            {
                int SizeDiv2 = Size / 2;
                Gr.DrawLine(Pen, Center.X - (float)SizeDiv2, Center.Y, Center.X + (float)SizeDiv2, Center.Y);
                Gr.DrawLine(Pen, Center.X, Center.Y - (float)SizeDiv2, Center.X, Center.Y + (float)SizeDiv2);
                return true;
            }
            catch (Exception Ex)
            {
                TLogging.Journalize(Ex);
                return false;
            }
        }

        /// <summary>
        /// Fills the solid ellipse.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Rect">The rect.</param>
        /// <param name="clrBack">The color back.</param>
        public static void FillSolidEllipse(Graphics gr, Rectangle Rect, Color clrBack)
        {
            m_FillSolidRectBrush.Color = clrBack;
            gr.FillEllipse(m_FillSolidRectBrush, Rect);
        }

        /// <summary>
        /// Fills the solid ellipse.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Rect">The rect.</param>
        /// <param name="clrBack">The color back.</param>
        /// <param name="clrBorder">The color border.</param>
        /// <param name="BorderWidth">Width of the border.</param>
        public static void FillSolidEllipse(Graphics gr, Rectangle Rect, Color clrBack, Color clrBorder, int BorderWidth)
        {
            m_FillSolidRectBrush.Color = clrBack;
            m_FillSolidRectPen.Color = clrBorder;
            m_FillSolidRectPen.Width = BorderWidth;
            gr.FillEllipse(m_FillSolidRectBrush, Rect);
            gr.DrawEllipse(m_FillSolidRectPen, Rect);
        }

        /// <summary>
        /// Fills the solid polygon.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="lpPoints">The lp points.</param>
        /// <param name="clrBack">The color back.</param>
        public static void FillSolidPolygon(Graphics gr, Point[] lpPoints, Color clrBack)
        {
            m_FillSolidRectBrush.Color = clrBack;
            gr.FillPolygon(m_FillSolidRectBrush, lpPoints);
        }

        /// <summary>
        /// Fills the solid polygon.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="lpPoints">The lp points.</param>
        /// <param name="clrBack">The color back.</param>
        /// <param name="clrBorder">The color border.</param>
        /// <param name="BorderWidth">Width of the border.</param>
        public static void FillSolidPolygon(Graphics gr, Point[] lpPoints, Color clrBack, Color clrBorder, int BorderWidth)
        {
            m_FillSolidRectBrush.Color = clrBack;
            m_FillSolidRectPen.Color = clrBorder;
            m_FillSolidRectPen.Width = BorderWidth;
            gr.FillPolygon(m_FillSolidRectBrush, lpPoints);
            gr.DrawPolygon(m_FillSolidRectPen, lpPoints);
        }

        /// <summary>
        /// Fills the solid rect.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Rect">The rect.</param>
        /// <param name="clrBack">The color back.</param>
        /// <param name="clrBorder">The color border.</param>
        /// <param name="BorderWidth">Width of the border.</param>
        public static void FillSolidRect(Graphics gr, Rectangle Rect, Color clrBack, Color clrBorder, int BorderWidth)
        {
            m_FillSolidRectPen.Color = clrBorder;
            m_FillSolidRectPen.Width = BorderWidth;
            m_FillSolidRectBrush.Color = clrBack;
            gr.FillRectangle(m_FillSolidRectBrush, Rect);
            gr.DrawRectangle(m_FillSolidRectPen, Rect);
        }

        /// <summary>
        /// Fills the solid rect.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Rect">The rect.</param>
        /// <param name="clrBack">The color back.</param>
        public static void FillSolidRect(Graphics gr, Rectangle Rect, Color clrBack)
        {
            m_FillSolidRectBrush.Color = clrBack;
            gr.FillRectangle(m_FillSolidRectBrush, Rect);
        }

        /// <summary>
        /// Gets the color of the dark.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        /// <returns>Color.</returns>
        public static Color GetDarkColor(this Color c, byte d)
        {
            if (d == 0) return c;
            byte r = (byte)(c.R < d ? 0 : c.R - d);
            byte g = (byte)(c.G < d ? 0 : c.G - d);
            byte b = (byte)(c.B < d ? 0 : c.B - d);
            return Color.FromArgb(c.A,r, g, b);
        }

        /// <summary>
        /// Gets the fitting font.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Text">The text.</param>
        /// <param name="Family">The family.</param>
        /// <param name="FS">The fs.</param>
        /// <param name="Rect">The rect.</param>
        /// <returns>Font.</returns>
        public static Font GetFittingFont(Graphics gr, string Text, string Family, FontStyle FS, Rectangle Rect)
        {
            Font Font = null;
            int I = 3000;
            try
            {
                SizeF FontSize;
                do
                {
                    I -= 1000;
                    if (I < 1)
                    {
                        I = 1;
                    }
                    Font?.Dispose();
                    Font = new Font(Family, I, FS);
                    FontSize = gr.MeasureString(Text, Font);
                }
                while (I > 1 && (FontSize.Width > (float)Rect.Width || FontSize.Height > (float)Rect.Height));
                I += 1000;
                do
                {
                    I -= 100;
                    if (I < 1)
                    {
                        I = 1;
                    }
                    Font?.Dispose();
                    Font = new Font(Family, I, FS);
                    FontSize = gr.MeasureString(Text, Font);
                }
                while (I > 1 && (FontSize.Width > (float)Rect.Width || FontSize.Height > (float)Rect.Height));
                I += 100;
                do
                {
                    I -= 10;
                    if (I < 1)
                    {
                        I = 1;
                    }
                    Font?.Dispose();
                    Font = new Font(Family, I, FS);
                    FontSize = gr.MeasureString(Text, Font);
                }
                while (I > 1 && (FontSize.Width > (float)Rect.Width || FontSize.Height > (float)Rect.Height));
                I += 10;
                do
                {
                    I--;
                    Font = new Font(Family, I, FS);
                    FontSize = gr.MeasureString(Text, Font);
                }
                while (I > 1 && (FontSize.Width > (float)Rect.Width || FontSize.Height > (float)Rect.Height));
            }
            catch (Exception Ex)
            {
                TLogging.Journalize(Ex);
            }
            return Font;
        }

        /// <summary>
        /// Gets the fitting font.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Text">The text.</param>
        /// <param name="OriginalFont">The original font.</param>
        /// <param name="Rect">The rect.</param>
        /// <returns>Font.</returns>
        public static Font GetFittingFont(Graphics gr, string Text, Font OriginalFont, Rectangle Rect)
        {
            Font Font = null;
            int I = 3000;
            try
            {
                I = (int)OriginalFont.Size;
                SizeF FontSize;
                do
                {
                    Font?.Dispose();
                    Font = new Font(OriginalFont.FontFamily, I, OriginalFont.Style);
                    FontSize = gr.MeasureString(Text, Font);
                    I--;
                }
                while (I > 1 && (FontSize.Width > (float)Rect.Width || FontSize.Height > (float)Rect.Height));
            }
            catch (Exception Ex)
            {
                TLogging.Journalize(Ex);
            }
            return Font;
        }

        /// <summary>
        /// Gets the color of the light.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        /// <returns>Color.</returns>
        public static Color GetLightColor(this Color c, byte d)
        {
            if (d == 0) return c;
            byte r = c.R + d < 255 ? (byte)(c.R + d) : byte.MaxValue;
            byte g = c.G + d < 255 ? (byte)(c.G + d) : byte.MaxValue;
            byte b = c.B + d < 255 ? (byte)(c.B + d) : byte.MaxValue;
            return Color.FromArgb(c.A, r, g, b);
        }

        /// <summary>
        /// Hollows the rect.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="Rect">The rect.</param>
        /// <param name="clrBorder">The color border.</param>
        /// <param name="BorderWidth">Width of the border.</param>
        public static void HollowRect(Graphics gr, Rectangle Rect, Color clrBorder, int BorderWidth)
        {
            m_FillSolidRectPen.Color = clrBorder;
            m_FillSolidRectPen.Width = BorderWidth;
            gr.DrawRectangle(m_FillSolidRectPen, Rect);
        }

        /// <summary>
        /// Makes the grayscale.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns>Bitmap.</returns>
        public static Bitmap MakeGrayscale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            Graphics g = Graphics.FromImage(newBitmap);
            ColorMatrix colorMatrix = new ColorMatrix(new float[5][]
            {
                new float[5]
                {
                    0.3f,
                    0.3f,
                    0.3f,
                    0f,
                    0f
                },
                new float[5]
                {
                    0.59f,
                    0.59f,
                    0.59f,
                    0f,
                    0f
                },
                new float[5]
                {
                    0.11f,
                    0.11f,
                    0.11f,
                    0f,
                    0f
                },
                new float[5]
                {
                    0f,
                    0f,
                    0f,
                    1f,
                    0f
                },
                new float[5]
                {
                    0f,
                    0f,
                    0f,
                    0f,
                    1f
                }
            });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();
            return newBitmap;
        }

        /// <summary>
        /// Makes the transparent.
        /// </summary>
        /// <param name="Src">The source.</param>
        /// <param name="X">The x.</param>
        /// <param name="Y">The y.</param>
        /// <returns>Bitmap.</returns>
        public static Bitmap MakeTransparent(Bitmap Src, int X = 0, int Y = 0)
        {
            Src.MakeTransparent(Src.GetPixel(X, Y));
            return Src;
        }

        /// <summary>
        /// Resizes the graphic.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <param name="MaxX">The maximum x.</param>
        /// <param name="MaxY">The maximum y.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static Bitmap ResizeGraphic(Bitmap Img, int MaxX, int MaxY)
        {
            if (Img.Width <= MaxX && Img.Height <= MaxY)
            {
                return Img;
            }
            int MaxSideDimension = (Img.Width <= Img.Height) ? MaxY : MaxX;
            double Ratio = (double)Img.Width / (double)Img.Height;
            return ((!(Ratio > 1.0)) ? new Bitmap(Img, new Size((int)((double)MaxSideDimension * Ratio), MaxSideDimension)) : new Bitmap(Img, new Size(MaxSideDimension, (int)((double)MaxSideDimension / Ratio))));
        }

        /// <summary>
        /// Rotates the image.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="AnglePoint">The angle point.</param>
        /// <param name="Angle">The angle.</param>
        /// <returns>Bitmap.</returns>
        public static Bitmap RotateImage(Bitmap b, Point AnglePoint, float Angle)
        {
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            Graphics g = Graphics.FromImage(returnBitmap);
            g.TranslateTransform(AnglePoint.X, AnglePoint.Y);
            g.RotateTransform(Angle);
            g.TranslateTransform(0f - (float)AnglePoint.X, 0f - (float)AnglePoint.Y);
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }

        /// <summary>
        /// Traffics the signal.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="R">The r.</param>
        /// <param name="Red">if set to <c>true</c> [red].</param>
        /// <param name="Yellow">if set to <c>true</c> [yellow].</param>
        /// <param name="Green">if set to <c>true</c> [green].</param>
        public static void TrafficSignal(Graphics gr, Rectangle R, bool Red, bool Yellow, bool Green)
        {
            Rectangle R2 = R;
            R2.Inflate(R.Width / 10, R.Width / 10);
            Rectangle rRed;
            Rectangle rYellow;
            Rectangle rGreen;
            if (R.Height > R.Width)
            {
                int x = R.X;
                int top = R.Top;
                _ = R.Height / 3;
                rRed = new Rectangle(x, top, R.Width, (int)Math.Ceiling((double)(R.Height - 2) / 3.0));
                rYellow = new Rectangle(R.X, R.Top + R.Height / 3, R.Width, (int)Math.Ceiling((double)(R.Height - 2) / 3.0));
                rGreen = new Rectangle(R.X, R.Top + R.Height / 3 * 2, R.Width, (int)Math.Ceiling((double)(R.Height - 2) / 3.0));
                Point P = new Point(rRed.X + rRed.Width / 2, rRed.Y + rRed.Height / 2);
            }
            else
            {
                double Delta = (R.Width - 3 * R.Height) / 2;
                int x2 = R.X + (int)Delta;
                _ = R.Height;
                rRed = new Rectangle(x2, R.Top, R.Height, R.Height);
                rYellow = new Rectangle(R.X + (int)Delta + R.Height, R.Top, R.Height, R.Height);
                rGreen = new Rectangle(R.X + (int)Delta + R.Height * 2, R.Top, R.Height, R.Height);
                Point P = new Point(rRed.X + rRed.Width / 2, rRed.Y + rRed.Height / 2);
            }
            FillSolidRect(gr, R2, Color.DarkGreen, Color.Black, R.Width / 10);
            if (Red)
                FillSolidEllipse(gr, rRed, Color.FromArgb(255, 0, 0));
            else
                FillSolidEllipse(gr, rRed, Color.White, Color.FromArgb(255, 0, 0), 1);
            if (Yellow)
                FillSolidEllipse(gr, rYellow, Color.FromArgb(255, 255, 0));
            else
                FillSolidEllipse(gr, rYellow, Color.White, Color.FromArgb(255, 200, 0), 1);
            if (Green)
                FillSolidEllipse(gr, rGreen, Color.FromArgb(0, 255, 0));
            else 
                FillSolidEllipse(gr, rGreen, Color.White, Color.FromArgb(0, 255, 0), 1);
        }

        /// <summary>
        /// Transforms the rectangle.
        /// </summary>
        /// <param name="gr">The gr.</param>
        /// <param name="destSpace">The dest space.</param>
        /// <param name="srcSpace">The source space.</param>
        /// <param name="Rect">The rect.</param>
        /// <returns>Rectangle.</returns>
        public static Rectangle TransformRectangle(Graphics gr, CoordinateSpace destSpace, CoordinateSpace srcSpace, Rectangle Rect)
        {
            Point[] Pts = new Point[2]
            {
                Rect.Location,
                new Point(Rect.Size.Width + Rect.Location.X, Rect.Size.Height + Rect.Location.Y)
            };
            gr.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, Pts);
            return new Rectangle(Pts[0].X, Pts[0].Y, Pts[1].X - Pts[0].X, Pts[1].Y - Pts[0].Y);
        }

        /// <summary>
        /// Calculates the led colors.
        /// </summary>
        /// <param name="OuterColor">Color of the outer.</param>
        /// <param name="BaseBrush">The base brush.</param>
        /// <param name="InnerBrush">The inner brush.</param>
        /// <param name="LEDIsActive">if set to <c>true</c> [led is active].</param>
        /// <param name="BaseColor">Color of the base.</param>
        private static (Brush bOuter,Brush bBase,Brush bInner) CalcLEDBrush(Color BaseColor, bool LEDIsActive)
        {
            var bBase = new SolidBrush(BaseColor);
            if (LEDIsActive)
            {
                return (new SolidBrush(GetDarkColor(BaseColor, 50)),bBase,new SolidBrush(GetLightColor(BaseColor, 100)));
            }
            else
            {
                return (new SolidBrush(GetDarkColor(BaseColor, 50)), new SolidBrush(GetDarkColor(BaseColor, 100)), new SolidBrush(GetDarkColor(BaseColor, 100)));
            }
        }
    }
}