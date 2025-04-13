using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGJKAlg.Views;

/// <summary>
/// Klasse V2 repräsentiert einen 3D-Vektor mit X, Y und Z Komponenten
/// </summary>
public class V2
{
    /// <summary>
    /// Eigenschaften für die X, Y und Z Komponenten des Vektors
    /// </summary>
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    /// <summary>
    /// Standardkonstruktor, der den Vektor auf (0, 0, 0) initialisiert
    /// </summary>
    public V2()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    /// <summary>
    /// Konstruktor, der den Vektor auf (x, y, 0) initialisiert
    /// </summary>
    /// <param name="x">X-Komponente</param>
    /// <param name="y">Y-Komponente</param>
    public V2(float x, float y)
    {
        X = x;
        Y = y;
        Z = 0;
    }

    /// <summary>
    /// Überladung des + Operators für die Addition von zwei Vektoren
    /// </summary>
    /// <param name="VectorA">Erster Vektor</param>
    /// <param name="VectorB">Zweiter Vektor</param>
    /// <returns>Ergebnisvektor</returns>
    public static V2 operator +(V2 VectorA, V2 VectorB) 
        => new V2(VectorA.X + VectorB.X, VectorA.Y + VectorB.Y);

    /// <summary>
    /// Überladung des + Operators für die Addition eines Skalars zu einem Vektor
    /// </summary>
    /// <param name="Vector">Vektor</param>
    /// <param name="Scalar">Skalar</param>
    /// <returns>Ergebnisvektor</returns>
    public static V2 operator +(V2 Vector, float Scalar) 
        => new V2(Vector.X + Scalar, Vector.Y + Scalar);

    /// <summary>
    /// Überladung des - Operators für die Subtraktion von zwei Vektoren
    /// </summary>
    /// <param name="VectorA">Erster Vektor</param>
    /// <param name="VectorB">Zweiter Vektor</param>
    /// <returns>Ergebnisvektor</returns>
    public static V2 operator -(V2 VectorA, V2 VectorB) 
        => new V2(VectorA.X - VectorB.X, VectorA.Y - VectorB.Y);

    /// <summary>
    /// Überladung des - Operators für die Subtraktion eines Skalars von einem Vektor
    /// </summary>
    /// <param name="Vector">Vektor</param>
    /// <param name="Scalar">Skalar</param>
    /// <returns>Ergebnisvektor</returns>
    public static V2 operator -(V2 Vector, float Scalar) 
        => new V2(Vector.X - Scalar, Vector.Y - Scalar);

    /// <summary>
    /// Überladung des * Operators für die Multiplikation von zwei Vektoren
    /// </summary>
    /// <param name="VectorA">Erster Vektor</param>
    /// <param name="VectorB">Zweiter Vektor</param>
    /// <returns>Ergebnisvektor</returns>
    public static V2 operator *(V2 VectorA, V2 VectorB) => new V2(VectorA.X * VectorB.X, VectorA.Y * VectorB.Y);

    /// <summary>
    /// Überladung des * Operators für die Multiplikation eines Vektors mit einem Skalar
    /// </summary>
    /// <param name="Vector">Vektor</param>
    /// <param name="Scalar">Skalar</param>
    /// <returns>Ergebnisvektor</returns>
    public static V2 operator *(V2 Vector, float Scalar) => new V2(Vector.X * Scalar, Vector.Y * Scalar);

    /// <summary>
    /// Überladung des / Operators für die Division von zwei Vektoren
    /// </summary>
    /// <param name="VectorA">Erster Vektor</param>
    /// <param name="VectorB">Zweiter Vektor</param>
    /// <returns>Ergebnisvektor</returns>
    public static V2 operator /(V2 VectorA, V2 VectorB) => new V2(VectorA.X / VectorB.X, VectorA.Y / VectorB.Y);

    /// <summary>
    /// Überladung des / Operators für die Division eines Vektors durch einen Skalar
    /// </summary>
    /// <param name="Vector">Vektor</param>
    /// <param name="Scalar">Skalar</param>
    /// <returns>Ergebnisvektor</returns>
    public static V2 operator /(V2 Vector, float Scalar) => new V2(Vector.X / Scalar, Vector.Y / Scalar);

    public Point Point => new Point((int)X, (int)Y);

    /// <summary>
    /// Methode zur Berechnung des Skalarprodukts (Dot-Produkt) von zwei Vektoren
    /// </summary>
    /// <param name="VectorA">Erster Vektor</param>
    /// <param name="VectorB">Zweiter Vektor</param>
    /// <returns>Skalarprodukt</returns>
    public static float Dot(V2 VectorA, V2 VectorB) => VectorA.X * VectorB.X + VectorA.Y * VectorB.Y;

    /// <summary>
    /// Methode zur Berechnung der Länge (Distanz) zwischen zwei Vektoren
    /// </summary>
    /// <param name="VectorA">Erster Vektor</param>
    /// <param name="VectorB">Zweiter Vektor</param>
    /// <returns>Länge</returns>
    public static float Length(V2 VectorA, V2 VectorB) => (float)Math.Sqrt(Math.Pow((VectorB.X - VectorA.X), 2) + Math.Pow((VectorB.Y - VectorA.Y), 2));

    /// <summary>
    /// Methode zur Berechnung des Kreuzprodukts von zwei Vektoren
    /// </summary>
    /// <param name="VectorA">Erster Vektor</param>
    /// <param name="VectorB">Zweiter Vektor</param>
    /// <returns>Kreuzprodukt</returns>
    public static V2 Cross(V2 VectorA, V2 VectorB)
    {
        V2 VtempRes = new V2();

        VtempRes.X = (VectorA.Y * VectorB.Z) - (VectorA.Z * VectorB.Y);
        VtempRes.Y = (VectorA.Z * VectorB.X) - (VectorA.X * VectorB.Z);
        VtempRes.Z = (VectorA.Z * VectorB.Y) - (VectorA.Y * VectorB.X);

        return VtempRes;
    }

    /// <summary>
    /// Methode zur Berechnung des inversen Vektors
    /// </summary>
    /// <returns>Inverser Vektor</returns>
    public V2 Inverse() => new V2(-1 * X, -1 * Y);

    public static V2 Zero => new V2();

    public static V2 EX => new V2(1,0);
    public static V2 EY => new V2(0, 1);
    public static V2 EZ => Cross(EX, EY);

    public override bool Equals(object obj) => obj is V2 o ? o.X == X && o.Y == Y : false;
}
