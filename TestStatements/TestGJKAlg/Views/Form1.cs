using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TestGJKAlg.Views;

namespace TestGJKAlg;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        Load += Form1_Load;
        KeyDown += Form1_KeyDown;
        KeyUp += Form1_KeyUp;
        Paint += Form1_Paint;
    }

    bool up = false;
    bool down = false;
    bool left = false;
    bool right = false;

    V2 ShapeDirection = new V2();

    List<V2> ShapeA = new List<V2>();
    List<V2> ShapeB = new List<V2>();

    private void Form1_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        V2 Direction = ShapeDirection;

        //Draw Shape A
        DrawShape(g, ShapeA, new Pen(Brushes.Black, 2));

        //Move Shape B
        for (int i = 0; i < ShapeB.Count; i++)
        {
            ShapeB[i] += Direction;
        }

        //Draw Shape B
        DrawShape(g, ShapeB, new Pen(Brushes.Black, 2));
        
        //Offset origin to see minkowski difference points
        g.TranslateTransform(900, 600);        
        g.FillEllipse(Brushes.Green, new Rectangle(-3, -3, 6, 6));

        List<V2> MDifference = Minkowski(ShapeA, ShapeB);

        //Draw minkowski difference points
        for (int i = 0; i < MDifference.Count - 1; i++)
        {
            g.FillEllipse(Brushes.Red,
                new Rectangle(MDifference[i].Point+new Size(-3,-3),new Size( 6, 6)));
        }
      //  DrawShape(g, MDifference, new Pen(Brushes.Red, 1));

        //Start of GJK
        if (Direction.Equals(V2.Zero)) return;
        V2 C = SupportPoint(ShapeA, Direction) - SupportPoint(ShapeB, Direction.Inverse());
        Direction = Direction.Inverse();
        V2 B = SupportPoint(ShapeA, Direction) - SupportPoint(ShapeB, Direction.Inverse());

        V2 Cline = B - C;
        V2 C0 = C.Inverse();

        Direction = new V2(Cline.Y, -Cline.X);
        if (V2.Dot(Direction,C) >0)
            Direction = Direction.Inverse();

        V2 A = SupportPoint(ShapeA, Direction) - SupportPoint(ShapeB, Direction.Inverse());

        g.DrawLine(new Pen(Brushes.Yellow, 2), new Point((int)A.X, (int)A.Y), new Point((int)B.X, (int)B.Y));

        g.DrawLine(new Pen(Brushes.Pink, 2), new Point((int)B.X, (int)B.Y), new Point((int)C.X, (int)C.Y));

        g.DrawLine(new Pen(Brushes.Yellow, 2), new Point((int)A.X, (int)A.Y), new Point((int)C.X, (int)C.Y));
    }

    private List<V2> GetShapeA()
    {
        return this.ShapeA;
    }

    private void DrawShape(Graphics g, List<V2> shape, Pen pen)
    {
        for (int i = 0; i < shape.Count; i++)
        {
            g.DrawLine(pen,
                shape[i].Point,
                shape[(i + 1)% shape.Count].Point);            
        }
    }

    private List<V2> Minkowski(List<V2> shape1, List<V2> shape2)
    {
        List<V2> MinkowskiDifferencePoints = new List<V2>();

        foreach (V2 VertexA in shape1)
        {
            foreach (V2 VertexB in shape2)
            {
                MinkowskiDifferencePoints.Add(VertexA - VertexB);
            }
        }

        return MinkowskiDifferencePoints;
    }

    public V2 SupportPoint(List<V2> shape, V2 direction)
    {
        float max=0;
        int index = 0;

        for (int i = 0; i < shape.Count; i++)
        {
            float dot = V2.Dot(shape[i], direction);
            if (i==0 || dot > max )
            {
                max = dot;
                index = i;
            }
        }

        return shape[index];
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.A) { left = true; }
        if (e.KeyCode == Keys.D) { right = true; }
        if (e.KeyCode == Keys.W) { up = true; }
        if (e.KeyCode == Keys.S) { down = true; }
    }

    private void Form1_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.A) { left = false; }
        if (e.KeyCode == Keys.D) { right = false; }
        if (e.KeyCode == Keys.W) { up = false; }
        if (e.KeyCode == Keys.S) { down = false; }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        timer1.Interval = 1;
        timer1.Enabled = true;
        timer1.Tick += timer1_Tick;
        this.DoubleBuffered = true;
        this.Size = new Size(1200, 1000);
        this.StartPosition = FormStartPosition.CenterScreen;

        ShapeA.Add(new V2(300, 300));
        ShapeA.Add(new V2(350, 300));
        ShapeA.Add(new V2(350, 350));
        ShapeA.Add(new V2(300, 350));

        ShapeB.Add(new V2(600, 600));
        ShapeB.Add(new V2(650, 650));
        ShapeB.Add(new V2(550, 650));
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        ShapeDirection = new V2();

        if (left) { ShapeDirection.X = -1; }
        else if (right) { ShapeDirection.X = 1; }
        if (up) { ShapeDirection.Y = -1; }
        else if (down) { ShapeDirection.Y = 1; }

        this.Invalidate();
    }
}