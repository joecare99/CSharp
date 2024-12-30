using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MathLibrary.TwoDim.Math2d;

namespace MathLibrary.TwoDim
{
    public record StTrackSeg(Vector vFootPoint, Vector vNormal, double lrRadius)
    {
        public StTrackSeg():this(Null,Null,double.NaN){ }
    }

    public class CProcAntennaValues
    {
        private Vector[] aPunkteSpeicher = new Vector[31];
        public Vector[] HMI = new Vector[33];
        private double fDispFak = 0.25;
        private double lrCfgDeichsellaenge = 1000.0;
        private double lrCfgAntennaOffs = 300.0;
        private double lrCfgEinfgDist = 20.0;
        private double lrCfgMinRadius = 500.0;
        private bool bflag = true;
        const double fAnpFakt = 0.05d;

        public class _Debug(CProcAntennaValues p)
        {
            CProcAntennaValues _p = p;
            public Vector[] aPoints => _p.aPunkteSpeicher;
            public bool bFlag => _p.bflag;
        }

        public _Debug Debug;

        public CProcAntennaValues() { Debug = new(this); }
        public CProcAntennaValues(Vector[] v) : this()
        {
            for (int i = 0; i < aPunkteSpeicher.Length; i++)
                if (i < v.Length)
                    aPunkteSpeicher[i] = v[i];
                else
                    aPunkteSpeicher[i] = new();
        }

        public bool Config(double lrCPOffset, double lrAntennaOffset, double lrEntryDist)
        {
            lrCfgDeichsellaenge = lrCPOffset;
            lrCfgAntennaOffs = lrAntennaOffset;
            lrCfgEinfgDist = lrEntryDist;
            return true;
        }

        /// <summary>Handles the movement.</summary>
        /// <param name="lrRot">The rotation [rad/s] ccw.</param>
        /// <param name="lrTrans">The (x-)translation [mm/s].</param>
        /// <param name="lrDeltaT">The time between the last call [s].</param>
        /// <returns>true if everything went well</returns>
        public bool HandleMovement(double lrRot, double lrTransl, double lrDeltaT)
        {
            var tfRotpoint = ByLengthAngle(1.0, -lrRot * lrDeltaT);
            for (int i = 0; i < aPunkteSpeicher.Length; i++)
            {
                var tfPoint = aPunkteSpeicher[i].Add(Vec(-lrTransl * lrDeltaT, 0.0));
                aPunkteSpeicher[i] = tfPoint.CMult(tfRotpoint);
                if (i < 25)
                    HMI[i] = aPunkteSpeicher[i].Mult(fDispFak);
            }
            return true;
        }

        public bool HandleStdAntennaValue(double lrAntennaValue, bool xAntDetect)
        {
            // Wert in Tabelle einfügen oder ändern
            var tfPoint = new Vector(lrCfgAntennaOffs, lrAntennaValue);
            var nIdx = 0;
            if (xAntDetect 
                && (((tfPoint.x - aPunkteSpeicher[0].x) > lrCfgEinfgDist)
                    || ((tfPoint.x - aPunkteSpeicher[1].x) > 2*lrCfgEinfgDist)))
            {
                for (var i = 30; i > 0; i--)
                    aPunkteSpeicher[i] = aPunkteSpeicher[i - 1];
                aPunkteSpeicher[0] = tfPoint;
                HMI[0] = aPunkteSpeicher[0].Mult(fDispFak);
            }
            else if (xAntDetect
                && (((tfPoint.x - aPunkteSpeicher[30].x) < -lrCfgEinfgDist) 
                    ||((tfPoint.x - aPunkteSpeicher[29].x) < -2*lrCfgEinfgDist)))
            {
                for (var i = 0; i < 30; i++)
                    aPunkteSpeicher[i] = aPunkteSpeicher[i + 1];
                bflag = false;
                nIdx = 30;
                aPunkteSpeicher[30] = tfPoint;
                HMI[30] = aPunkteSpeicher[30].Mult(fDispFak);
            }
            else if (xAntDetect)
            {
                var fDist = 0d;
                for (var i = 0; i < 31; i++)
                    if ((i == 0) || (Math.Abs(aPunkteSpeicher[i].x - tfPoint.x) < fDist))
                    {
                        fDist = Math.Abs(aPunkteSpeicher[i].x - tfPoint.x);
                        nIdx = i;
                    }
                aPunkteSpeicher[nIdx] =
                        aPunkteSpeicher[nIdx]
                        .Mult(1.0 - fAnpFakt)
                        .Add(tfPoint.Mult(fAnpFakt));
            }
            return true;
        }

        public bool ComputeTrackAndLookAhead(double lrRot, double lrTransl, double lrLHTime,out StTrackSeg stTrack,out StTrackSeg stLHTrack,out double fYDeviation)
        {
            Calculate3DistinctPoints(out Vector tfPunktVorne, out Vector tfPunktMitte, out Vector tfPunktHinten);

            Vector[] av = CalculateLookAhead(lrRot, lrTransl, lrLHTime, [tfPunktVorne, tfPunktMitte, tfPunktHinten]);
            var tfLHPunktVorne = av[0];
            var tfLHPunktMitte = av[1];
            var tfLHPunktHinten = av[2];

            stTrack = CalcTrack([tfPunktVorne, tfPunktMitte, tfPunktHinten], out Vector vCenter,out fYDeviation);
            HMI[29] = vCenter.Mult(fDispFak);

            stLHTrack = CalcTrack([tfLHPunktVorne, tfLHPunktMitte, tfLHPunktHinten], out _, out _);
            return true;
        }

        public StTrackSeg CalcTrack(Vector[] value, out Vector vCenter, out double fDist)
        {
            StTrackSeg stResult = new();
            fDist = double.NaN;
            vCenter = CircleCenter(value, out double fRadius);
            if (fRadius != double.PositiveInfinity
                && Math.Abs(fRadius) < 6000.0
                && Math.Abs(fRadius) > lrCfgMinRadius)
            {
                fDist = vCenter.Length() - Math.Abs(fRadius);
                var vNormal = vCenter.Mult(-1.0 / (Math.Abs(fRadius) + fDist));
                stResult = new(vNormal.Mult(-fDist), vNormal, fRadius);
            }
            else
            {
                fRadius = 0d;
                var vNormal = value[0].Subtract(value[2]);
                var lrNormalLength = vNormal.Length();
                if (Math.Abs(lrNormalLength) > 1E-8)
                {
                    vNormal = vNormal.Rot90().Mult(1.0 / lrNormalLength);
                    fDist = -vNormal.Mult(value[0]);
                    stResult = new(vNormal.Mult(-fDist), vNormal, fRadius);
                }
                else
                {
                    stResult = new(value[0], Null, fRadius);
                }
            }
            return stResult;
        }

        public static Vector[] CalculateLookAhead(double lrRot, double lrTransl, double lrLHTime, Vector[] value1)
        {
            var tfRotpoint = ByLengthAngle(1.0, -lrRot * lrLHTime);
            var tfRotpoint2 = ByLengthAngle(AGVHandling.SinX_X(-lrRot * lrLHTime * 0.5), -lrRot * lrLHTime * 0.5);
            var tfVecMov = tfRotpoint2.CMult(new(-lrTransl * lrLHTime, 0));
            var vResult = new Vector[value1.Length];
            for (var i = 0; i < value1.Length; i++)
                vResult[i] = tfVecMov.Add(tfRotpoint.CMult(value1[i]));
            return vResult;
        }

        public void Calculate3DistinctPoints(out Vector tfPunktVorne, out Vector tfPunktMitte, out Vector tfPunktHinten)
        {
            tfPunktVorne = Null;
            tfPunktMitte = Null;
            tfPunktHinten = Null;
            for (var i = 0; i < 16; i++)
            {
                tfPunktVorne = tfPunktVorne.Add(aPunkteSpeicher[i]);
                tfPunktMitte = tfPunktMitte.Add(aPunkteSpeicher[i + 7]);
                tfPunktHinten = tfPunktHinten.Add(aPunkteSpeicher[i + 14]);
            };
            tfPunktVorne = tfPunktVorne.Mult(0.0625);
            tfPunktMitte = tfPunktMitte.Mult(0.0625);
            tfPunktHinten = tfPunktHinten.Mult(0.0625);
            HMI[25] = tfPunktVorne.Mult(fDispFak);
            HMI[26] = tfPunktHinten.Mult(fDispFak);
        }

        public double ComputeVAntennaVal(StTrackSeg stTrack,bool xRueckwaerts = false)
        {
            var tFussPunkt = stTrack.vFootPoint;
            HMI[28] = tFussPunkt.Mult(fDispFak);
            Vector tZielPunkt;
            if (Math.Abs(stTrack.lrRadius) >1e-8)
            {
                tZielPunkt =
                    ByLengthAngle(lrCfgDeichsellaenge *
                Math.Abs(stTrack.lrRadius) / stTrack.lrRadius *
                (xRueckwaerts ? -1.0 : 1.0),
                0.25 * lrCfgDeichsellaenge / stTrack.lrRadius *
                (xRueckwaerts ? -1.0 : 1.0) + pi * 0.5).CMult(stTrack.vNormal);
                tZielPunkt = tFussPunkt.Add(tZielPunkt);
                HMI[27] = tZielPunkt.Mult(fDispFak);
            }
            else
            {
                var lrNormalLength = stTrack.vNormal.Length();
                if (Math.Abs(lrNormalLength) > 1E-8)
                {
                    tZielPunkt =
                        tFussPunkt.Add(stTrack.vNormal.Rot90().Mult(-lrCfgDeichsellaenge * (xRueckwaerts ? -1.0 : 1.0)));
                    HMI[27] = tZielPunkt.Mult(fDispFak);
                }
                else
                    return 0d;

            }

            if (xRueckwaerts)
                tZielPunkt = tZielPunkt.Add(new Vector(lrCfgDeichsellaenge * 0.5, 0));
            else
                tZielPunkt = tZielPunkt.Add(new Vector(-lrCfgDeichsellaenge * 0.5, 0));


            Math2d.TryLengthAngle(tZielPunkt, out var fLenkWinkel, out _);
            if (xRueckwaerts)
                fLenkWinkel = -fLenkWinkel;

            return fLenkWinkel;
        }
    }
}
