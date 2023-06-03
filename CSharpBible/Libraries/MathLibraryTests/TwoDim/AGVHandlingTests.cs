using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathLibrary.TwoDim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary.TwoDim.Tests
{
    [TestClass()]
    public class AGVHandlingTests
    {

        [DataTestMethod()]
        [DataRow(0, 0, 1)]
        [DataRow(0.5 * Math.PI, -0.25, 0.6366197723675814)]
        [DataRow(-0.5 * Math.PI, 0.25, 0.6366197723675814)]
        [DataRow(Math.PI, -1, 0)]
        [DataRow(-Math.PI, 1, 0)]
        [DataRow(1.5 * Math.PI, 0, -0.2122065907891938)]
        [DataRow(-1.5 * Math.PI, 0, -0.2122065907891938)]
        [DataRow(2 * Math.PI, 0.5, 0)]
        [DataRow(-2 * Math.PI, -0.5, 0)]
        public void SinX_XTest(double x, double dx, double dExp)
        {
            const double _eps = 1e-10;
            Assert.AreEqual(dExp, AGVHandling.SinX_X(x), 1e-8, $"SinX_X({x}) == {dExp}");
            Assert.AreEqual(dExp + dx * _eps, AGVHandling.SinX_X(x + _eps), 1e-8, $"SinX_X({x + _eps}) == {dExp} (+e)");
            Assert.AreEqual(dExp - dx * _eps, AGVHandling.SinX_X(x - _eps), 1e-8, $"SinX_X({x - _eps}) == {dExp} (-1)");
        }

        [DataTestMethod()]
        [DataRow(0, 0, 0,0,0,0,0)]
        [DataRow(0, 0, 0, 1, 0, 0, 0)]
        [DataRow(0, 0, 0, 2, 0, 0, 0)]
        [DataRow(1, 0, 0, 0, 0, 0, 0)]
        [DataRow(1, 0, 0, 1, 1, 0, 0)]
        [DataRow(1, 0, 0, 2, 2, 0, 0)]
        [DataRow(1, 1d / 6d * Math.PI, 0, 0, 0, 0, 0)]
        [DataRow(1, 1d / 6d * Math.PI, 0, 1, 0.8660254037844387d, 0.5, 0)]
        [DataRow(1, 1d / 6d * Math.PI, 0, 2, 2*0.8660254037844387d, 1, 0)]
        [DataRow(1, 0.25 * Math.PI, 0, 0, 0, 0, 0)]
        [DataRow(1, 0.25 * Math.PI, 0, 1, 0.7071067811865476, 0.7071067811865476, 0)]
        [DataRow(1, 0.25 * Math.PI, 0, 2, 2*0.7071067811865476, 2*0.7071067811865476, 0)]
        [DataRow(1, 1d / 3d * Math.PI, 0, 0, 0, 0, 0)]
        [DataRow(1, 1d / 3d * Math.PI, 0, 1, 0.5, 0.8660254037844387d, 0)]
        [DataRow(1, 1d / 3d * Math.PI, 0, 2, 1, 2* 0.8660254037844387d, 0)]
        [DataRow(1, 0.5 * Math.PI, 0, 0, 0, 0, 0)]
        [DataRow(1, 0.5 * Math.PI, 0, 1, 0, 1, 0)]
        [DataRow(1, 0.5 * Math.PI, 0, 2, 0, 2, 0)]
        [DataRow(1, 0, 1d / 6d * Math.PI, 0, 0, 0, 0)]
        [DataRow(1, 0, 1d / 6d * Math.PI, 1, 0.954929658551372, 0.255872630837368, 0.523598775598299)]
        [DataRow(1, 0, 1d / 6d * Math.PI, 2, 1.65398668626538, 0.954929658551372, 1.0471975511966)]
        public void AGVStateTest(double l, double vr, double r, double t, double xPx, double xPy, double xAng)
        {
            var v = Math2d.ByLengthAngle(l, vr);
            AGVHandling.AGVState(v, r, t, out Math2d.Vector vPos, out double ang);
            Assert.AreEqual(xPx, vPos.x, 1e-8, $"({vPos.x};{vPos.y}).x == {xPx}");
            Assert.AreEqual(xPy, vPos.y, 1e-8, $"({vPos.x};{vPos.y}).y == {xPy}");
            Assert.AreEqual(xAng,ang, 1e-8, $"Angle({ang}) == {xAng}");
        }
    }
}