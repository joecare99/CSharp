// ***********************************************************************
// Assembly         : MathLibrary_net
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-11-2022
// ***********************************************************************
// <copyright file="AGVHandling.cs" company="MathLibrary_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace MathLibrary.TwoDim
{
	/// <summary>
	/// Class AGVHandling.
	/// </summary>
	public static class AGVHandling {
		/// <summary>
		/// Stetic function of Sine(x)/x.
		/// </summary>
		/// <param name="x">The x value.</param>
		/// <returns>Value of the stetic function</returns>
		public static double SinX_X(this double x) => Math.Abs(x) > 1e-8 ? Math.Sin(x) / x : Math.Cos(0.5 * x);

		/// <summary>
		/// Computes the (new) position and angle (in AGV-coordinates) in the given time.
		/// </summary>
		/// <param name="velocity">The velocity of the AGV (in AGV-coordinates).</param>
		/// <param name="rotation">The rotation of the AGV [rad/s].</param>
		/// <param name="time">The time [s].</param>
		/// <param name="position">The new position (in AGV-coordinates).</param>
		/// <param name="angle">The new angle [rad] (in AGV-coordinates).</param>
		public static void AGVState(Math2d.Vector velocity, double rotation, double time, out Math2d.Vector position, out double angle) {
			position = Math2d.Mult(Math2d.Rotate(velocity, 0.5 * rotation * time), SinX_X(0.5 * rotation * time) * time);
			angle = rotation * time;
		}

		/// <summary>
		/// Agvs the steering.
		/// </summary>
		/// <param name="velocity">The velocity.</param>
		/// <param name="rotation">The rotation.</param>
		/// <param name="steerpos">The steerpos.</param>
		/// <param name="strvelocity">The strvelocity.</param>
		public static void AGVSteering(Math2d.Vector velocity, double rotation, Math2d.Vector steerpos, out Math2d.Vector strvelocity) {
			strvelocity = Math2d.Add(velocity, Math2d.Mult(Math2d.Rot90(steerpos), rotation));
		}

	}
}
