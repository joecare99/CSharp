// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 07-13-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="SwitchStatement2.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class Shape.
    /// </summary>
    public abstract class Shape
    {
        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <value>The area.</value>
        public abstract double Area { get; }
        /// <summary>
        /// Gets the circumference.
        /// </summary>
        /// <value>The circumference.</value>
        public abstract double Circumference { get; }
    }

    /// <summary>
    /// Class Rectangle.
    /// Implements the <see cref="TestStatements.Anweisungen.Shape" />
    /// </summary>
    /// <seealso cref="TestStatements.Anweisungen.Shape" />
    public class Rectangle : Shape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle" /> class.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="width">The width.</param>
        public Rectangle(double length, double width)
        {
            Length = length;
            Width = width;
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public double Length { get; set; }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width { get; set; }

        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <value>The area.</value>
        public override double Area
        {
            get { return Math.Round(Length * Width, 2); }
        }

        /// <summary>
        /// Gets the circumference.
        /// </summary>
        /// <value>The circumference.</value>
        public override double Circumference
        {
            get { return (Length + Width) * 2; }
        }
    }

    /// <summary>
    /// Class Square.
    /// Implements the <see cref="TestStatements.Anweisungen.Rectangle" />
    /// </summary>
    /// <seealso cref="TestStatements.Anweisungen.Rectangle" />
    public class Square : Rectangle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Square" /> class.
        /// </summary>
        /// <param name="side">The side.</param>
        public Square(double side) : base(side, side)
        {
            Side = side;
        }

        /// <summary>
        /// Gets or sets the side.
        /// </summary>
        /// <value>The side.</value>
        public double Side { get; set; }
    }

    /// <summary>
    /// Class Circle.
    /// Implements the <see cref="TestStatements.Anweisungen.Shape" />
    /// </summary>
    /// <seealso cref="TestStatements.Anweisungen.Shape" />
    public class Circle : Shape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Circle" /> class.
        /// </summary>
        /// <param name="radius">The radius.</param>
        public Circle(double radius)
        {
            Radius = radius;
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public double Radius { get; set; }

        /// <summary>
        /// Gets the circumference.
        /// </summary>
        /// <value>The circumference.</value>
        public override double Circumference
        {
            get { return 2 * Math.PI * Radius; }
        }

        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <value>The area.</value>
        public override double Area
        {
            get { return Math.PI * Math.Pow(Radius, 2); }
        }
    }

    /// <summary>
    /// Class SwitchStatement2.
    /// </summary>
    public class SwitchStatement2
    {
        /// <summary>
        /// Switches the example21.
        /// </summary>
        public static void SwitchExample21()
        {
#if NET5_0_OR_GREATER
			Shape? sh = null;
#else
			Shape sh = null;
#endif
            Shape[] shapes = { new Square(10), new Rectangle(5, 7),
                         sh, new Square(0), new Rectangle(8, 8),
                         new Circle(3) };
            foreach (var shape in shapes)
                ShowShapeInfo(shape);
        }

        /// <summary>
        /// Shows the shape information.
        /// </summary>
        /// <param name="sh">The sh.</param>
        public static void ShowShapeInfo(Shape sh)
        {
            switch (sh)
            {
                // Note that this code never evaluates to true.
                case Shape shape when shape == null:
                    Console.WriteLine($"An uninitialized shape (shape == null)");
                    break;
                case null:
                    Console.WriteLine($"An uninitialized shape");
                    break;
                case Shape shape when sh.Area == 0:
                    Console.WriteLine($"The shape: {sh.GetType().Name} with no dimensions");
                    break;
                case Square sq when sh.Area > 0:
                    Console.WriteLine("Information about square:");
                    Console.WriteLine($"   Length of a side: {sq.Side}");
                    Console.WriteLine($"   Area: {sq.Area}");
                    break;
                case Rectangle r when r.Length == r.Width && r.Area > 0:
                    Console.WriteLine("Information about square rectangle:");
                    Console.WriteLine($"   Length of a side: {r.Length}");
                    Console.WriteLine($"   Area: {r.Area}");
                    break;
                case Rectangle r when sh.Area > 0:
                    Console.WriteLine("Information about rectangle:");
                    Console.WriteLine($"   Dimensions: {r.Length} x {r.Width}");
                    Console.WriteLine($"   Area: {r.Area}");
                    break;
                case Shape shape when sh != null:
                    Console.WriteLine($"A {sh.GetType().Name} shape");
                    break;
                default:
                    Console.WriteLine($"The {nameof(sh)} variable does not represent a Shape.");
                    break;
            }
        }
    }
}
