﻿using static System.Math;

namespace GenFree.Helper;

public static class PlaceHelpers
{
    private const double cEarthradius = 6378.388; //[km]
    private const double cRadDegr = PI / 180;

    /// <summary>   
    /// Calculates the distance.
    /// </summary>
    /// <param name="dLonA">The d lon a.</param>
    /// <param name="dLatA">The d lat a.</param>
    /// <param name="dLonB">The d lon b.</param>
    /// <param name="dLatB">The d lat b.</param>
    /// <returns>System.Single.</returns>
    /// <autogeneratedoc />
    public static float CalcDistance(float dLonA, float dLatA, float dLonB, float dLatB)
    {
        float fLongARad = (float)((double)dLonA * cRadDegr);
        float fLatARad = (float)((double)dLatA * cRadDegr);
        float fLongBRad = (float)((double)dLonB * cRadDegr);
        float fLatBRad = (float)((double)dLatB * cRadDegr);
        double fDist = Sin(fLatARad) * Sin(fLatBRad) + Cos(Abs(fLongARad - fLongBRad)) * Cos(fLatARad) * Cos(fLatBRad);
        fDist = Max(-1.0, Min(fDist, 1.0));
        var fLength = !(0.0 - Log(Abs(fDist) - 1.0) / Log(10.0) > 12.0)
                ? (Atan((0.0 - fDist) / Sqrt((0.0 - fDist) * fDist + 1.0)) + 0.5f * PI) * cEarthradius
                : 0.0;
        return (float)fLength;
    }
}