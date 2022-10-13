// ***********************************************************************
// Assembly         : MathLibrary_net
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-11-2022
// ***********************************************************************
// <copyright file="Math2d.cs" company="MathLibrary_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Numerics;

namespace MathLibrary.TwoDim {
	/// <summary>
	/// Class Math2d.
	/// </summary>
	public static class Math2d {

		/// <summary>
		/// Class Vector.
		/// </summary>
		public class Vector : object {
			/// <summary>
			/// The x
			/// </summary>
			private double _x, _y;

			/// <summary>
			/// Initializes a new instance of the <see cref="Vector" /> class.
			/// </summary>
			public Vector() {
				_x = 0;
				_y = 0;
			}
			/// <summary>
			/// Initializes a new instance of the <see cref="Vector" /> class.
			/// </summary>
			/// <param name="x">The x parameter.</param>
			/// <param name="y">The y parameter.</param>
			public Vector(double x, double y) {
				_x = x;
				_y = y;
			}

			/// <summary>
			/// Gets or sets the x.
			/// </summary>
			/// <value>The x.</value>
			public double x { get => _x; set => _x = value; }
			/// <summary>
			/// Gets or sets the y.
			/// </summary>
			/// <value>The y.</value>
			public double y { get => _y; set => _y = value; }

			/// <summary>
			/// Gets or sets the vector as a tuple.
			/// </summary>
			/// <value>The vector as a tuple.</value>
			public ValueTuple<double, double> vTuple { get => new ValueTuple<double, double>(_x, _y); set => (_x, _y) = value; }
			/// <summary>
			/// Gets or sets the vector as a complex number.
			/// </summary>
			/// <value>As complex.</value>
			public Complex AsComplex { get => new Complex(_x, _y); set => (_x, _y) = (value.Real, value.Imaginary); }

			/// <summary>
			/// Calculates the (carthesian) length of the vector.
			/// </summary>
			/// <returns>The (carthesian) length of the vector</returns>
			public double Length() => Math.Sqrt(_x * _x + _y * _y);

			/// <summary>
			/// Converts the vector to a string.
			/// </summary>
			/// <returns>Eine Zeichenfolge, die das aktuelle Objekt darstellt.</returns>
			public override string ToString() => $"( {_x}, {_y})";
			/// <summary>
			/// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
			public override bool Equals(object obj) => obj is Vector vector && _x == vector._x && _y == vector._y;

			/// <summary>
			/// Returns a hash code for this instance.
			/// </summary>
			/// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
			public override int GetHashCode() => _x.GetHashCode() ^ _y.GetHashCode();
		}

		/// <summary>
		/// Gets the Vector with the  length of 0.
		/// </summary>
		/// <value>(0,0)</value>
		public static Vector Null => new Vector();
		/// <summary>
		/// Gets the Vector with the  length of 1 in x-Direction.
		/// </summary>
		/// <value>(1,0)</value>
		public static Vector eX => new Vector(1, 0);
		/// <summary>
		/// Gets the Vector with the  length of 1 in y-Direction.
		/// </summary>
		/// <value>(0,1)</value>
		public static Vector eY => new Vector(0, 1);

		/// <summary>
		/// Adds the specified v1 to v2.
		/// </summary>
		/// <param name="v1">The v1.</param>
		/// <param name="v2">The v2.</param>
		/// <returns>(v1 + v2)</returns>
		public static Vector Add(Vector v1, Vector v2) => new Vector(v1.x + v2.x, v1.y + v2.y);
		/// <summary>
		/// Subtracts the specified v2 from v1 .
		/// </summary>
		/// <param name="v1">The v1.</param>
		/// <param name="v2">The v2.</param>
		/// <returns>the value of v1 - v2</returns>
		public static Vector Subtract(Vector v1, Vector v2) => new Vector(v1.x - v2.x, v1.y - v2.y);

		/// <summary>
		/// The constant value pi (3.1415...)
		/// </summary>
		public static readonly double pi = Math.Atan(1d) * 4d;

		/// <summary>
		/// Tries to compute the length and angle of the vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="length">The length of the vector.</param>
		/// <param name="angle">The angle of the vector [rad].</param>
		/// <returns>[True]: if the length and angle can be computed<br />[False]: otherwise<br /></returns>
		public static bool TryLengthAngle(Vector vector, out double length, out double angle) {
			length = vector.Length();
			angle = 0.0d;
			if (length == 0d)
				return false;
			if (Math.Abs(vector.x) >= Math.Abs(vector.y))
				// -45° to 45° und 135° to 225° 
				if (vector.x > 0)
					// -44.9..° to 44.9..°  
					if (vector.y >= 0)
						// 0 to 44.9..° und 
						angle = Math.Atan(vector.y / vector.x);
					else
						// -44.9 to -0.0..1° und 
						angle = 2 * pi + Math.Atan(vector.y / vector.x);

				else if (vector.x < 0)
					// 135.0..° to 224.9..° 
					angle = pi + Math.Atan(vector.y / vector.x);
				else // X = 0.0 & Y = 0.0
					angle = 0d;
			else
				// 45.0..1° to 134.9..° und 225.0..1° to 315.9...° 	
				if (vector.y > 0)
				// 45.0..1° to 134.9..° 
				angle = 0.5 * pi - Math.Atan(vector.x / vector.y);
			else
				// 225.0..1° to o 315.9...° 
				angle = 1.5 * pi + Math.Atan(-vector.x / vector.y);
			return true;
		}

		/// <summary>
		/// Rotates the specified vector v1 90° counter-clock-wise.
		/// </summary>
		/// <param name="v1">The vector.</param>
		/// <returns>(-v1.y,v1.x) the vector rotated 90° ccw.</returns>
		public static Vector Rot90(Vector v1) => new Vector(-v1.y, v1.x);

		/// <summary>
		/// Negates the v. Or rotates the specified vector v1 by 180°.
		/// </summary>
		/// <param name="v1">The vector.</param>
		/// <returns>(-v1.x,-v1.y) the negative vector or rotated by 180°.</returns>
		public static Vector Negate(Vector v1) => new Vector(-v1.x, -v1.y);

		/// <summary>
		/// Gives the specified vector by length and angle
		/// </summary>
		/// <param name="length">The length of the vector.</param>
		/// <param name="angle">The angle of the vector[rad].</param>
		/// <returns>The specified vector.</returns>
		public static Vector ByLengthAngle(double length, double angle) => new Vector(length * Math.Cos(angle), length * Math.Sin(angle));

		/// <summary>
		/// "Normalizes" the &lt;angle&gt; around &lt;middle&gt;
		/// </summary>
		/// <param name="angle">The angle [rad].</param>
		/// <param name="middle">The middle value.</param>
		/// <returns>if no middle is given the result will be 0 &lt;= &lt;result&gt; &lt; 2 * pi<br /> otherwise the result will be <param name="middle">&lt;middle&gt;</param> - pi &lt;= &lt;result&gt; &lt; <param name="middle">&lt;middle&gt;</param> + pi<br /></returns>
		public static double WinkelNorm(double angle, double middle = Math.PI) =>
			angle - Math.Floor((angle - middle + pi) / (2d * pi)) * (2d * pi);

		/// <summary>
		/// Multiplies the specified vector by the specified scalar.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="scalar">The scalar.</param>
		/// <returns>The result vector</returns>
		public static Vector Mult(Vector vector, double scalar)
			=> new Vector(scalar * vector.x, scalar * vector.y);
		/// <summary>
		/// Multiplies the specified vectors fak1 and fak2.
		/// </summary>
		/// <param name="fak1">The vector fak1.</param>
		/// <param name="fak2">The vector fak2.</param>
		/// <returns>The vector-product</returns>
		public static double Mult(Vector fak1, Vector fak2)
			=> fak1.x * fak2.x + fak1.y * fak2.y;
		/// <summary>
		/// Multiplies the specified vectors, if they were complex numbers.
		/// </summary>
		/// <param name="fak1">The vector fak1.</param>
		/// <param name="fak2">The vector fak2.</param>
		/// <returns>The complex product as vector</returns>
		public static Vector CMult(Vector fak1, Vector fak2)
			=> new Vector(fak1.x * fak2.x - fak1.y * fak2.y, fak1.x * fak2.y + fak1.y * fak2.x);
		/// <summary>
		/// Divides the specified vector by the scalar (divisor)
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="scalar">The scalar.</param>
		/// <returns>The quotient as a vector</returns>
		public static Vector Div(Vector vector, double scalar)
			=> new Vector(vector.x / scalar, vector.y / scalar);
		/// <summary>
		/// Rotates the specified vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="angle">The angle [rad].</param>
		/// <returns>The rotated vector</returns>
		public static Vector Rotate(Vector vector, double angle)
			=> new Vector(vector.x * Math.Cos(angle) - vector.y * Math.Sin(angle),
						 vector.y * Math.Cos(angle) + vector.x * Math.Sin(angle));

	}
}
