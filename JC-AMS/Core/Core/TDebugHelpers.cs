using JCAMS.Core.Logging;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="T.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace JCAMS.Core
{
    public static class TDebugHelpers
    {

        /// <summary>
        /// Gets the method stack.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetMethodStack()
        {
            StackTrace StackTrace = new StackTrace();
            int FrameCount = Math.Min(10, StackTrace.FrameCount);
            StringBuilder Stack = new StringBuilder();
            for (int I = FrameCount - 1; I > 1; I--)
            {
                try
                {
                    StackFrame StackFrame = StackTrace.GetFrame(I);
                    MethodBase MethodBase = StackFrame.GetMethod();
                    if (I < FrameCount)
                    {
                        Stack.Append(".");
                    }
                    Stack.Append(MethodBase.Name);
                }
                catch (Exception Ex)
                {
                    TLogging.Log(Ex);
                }
            }
            return Stack.ToString();
        }
    }
}