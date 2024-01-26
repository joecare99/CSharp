﻿// ***********************************************************************
// Assembly         : BaseLibTests
// Author           : Mir
// Created          : 03-28-2023
//
// Last Modified By : Mir
// Last Modified On : 03-28-2023
// ***********************************************************************
// <copyright file="TestHelper.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

/// <summary>
/// The Helper namespace.
/// </summary>
/// <autogeneratedoc />
namespace BaseLib.Helper
{
    /// <summary>
    /// Class TestHelper.
    /// </summary>
    /// <autogeneratedoc />
    public static class TestHelper
    {
        /// <summary>
        /// Asserts that both classes are equal. 
        /// string -> use String-Array proc
        /// with normal class compare fields & properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ExpData">The exp data.</param>
        /// <param name="ActData">The act data.</param>
        /// <param name="Excl">The excluded Properties</param>
        /// <param name="Msg">The MSG.</param>
        /// <autogeneratedoc />
        public static void AssertAreEqual<T>(T ExpData, T ActData, string[]? Excl, string Msg = "") where T : class
        {
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
                if (prop.CanRead && (Excl?.Contains(prop.Name) == false))
                {
                    Assert.AreEqual(ExpData.GetProp(prop.Name), ActData.GetProp(prop.Name), Msg + $".{prop.Name}");
                }
            var fields = typeof(T).GetFields();
            foreach (var field in fields)
                if (field.IsPublic && (Excl?.Contains(field.Name) == false))
                {
                    Assert.AreEqual(ExpData.GetField(field.Name), ActData.GetField(field.Name), Msg + $".{field.Name}");
                }
        }

        public static void AssertAreEqual<T>(T exp, T act, string Msg = "") where T : IEnumerable
        {
            static string BldLns(int i, string[] aLines)
                => (i > 1 ? $"#{i - 2:D3}: {aLines[(i + 3) % 5]}{Environment.NewLine}" : "") +
                   (i > 0 ? $"#{i - 1:D3}: {aLines[(i + 4) % 5]}{Environment.NewLine}" : "") +
                   $"#{i:D3}> {aLines[i % 5]}" +
                   (i < aLines.Length - 1 ? $"{Environment.NewLine}#{i + 1:D3}: {aLines[(i + 1) % 5]}" : "") +
                   (i < aLines.Length - 2 ? $"{Environment.NewLine}#{i + 2:D3}: {aLines[(i + 2) % 5]}" : "");

            var actE = act.GetEnumerator();
            var actLines = new string[5];
            var expLines = new string[5];
            actLines.Initialize();
            expLines.Initialize();
            var xEoAAct = !actE.MoveNext();
            var i = 0;
            var iErr = -1;
            foreach (var el in exp)
            {
                var al = !xEoAAct ?actE.Current:null;
                actLines[i % 5] = $"{(al is string or null ? "" : al.GetType())} {al}";
                expLines[i % 5] = $"{(el is string or null ? "" : el.GetType())} {el}";
                if (iErr == -1 && !el!.Equals(al))
                    iErr = i;
                if (iErr != -1 && i-3==iErr)
                    Assert.AreEqual(BldLns(iErr, expLines), BldLns(iErr, actLines), $"{Msg}: Entry{i}:");
                xEoAAct = !actE.MoveNext();                    
                i++;
            }
            if (iErr != -1 || (i==0 && !xEoAAct))
            {
                if (iErr == -1)
                {
                    iErr = i;
                    var al = !xEoAAct ? actE.Current : null;
                    actLines[i % 5] = $"{(al is string or null ? "" : al.GetType())} {al}";
                }
                else
                    actLines[i % 5] = $"";
                expLines[i % 5] = $"<EOF>";
                Assert.AreEqual(BldLns(iErr, expLines), BldLns(iErr, actLines), $"{Msg}: Entry{i}:");  }
            else
            { 
                Assert.IsTrue(xEoAAct);
            }

        }
        /// <summary>
        /// Assert that both string-arrays are equal. (with diagnosis)
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <param name="act">The act.</param>
        /// <param name="Msg">The MSG.</param>
        /// <autogeneratedoc />
        public static void AssertAreEqual(string[] exp, string[] act, string Msg = "")
        {
            static string BldLns(int i, string[] aLines)
                => (i > 1 ? $"#{i - 2:D3}: {aLines[i - 2]}{Environment.NewLine}" : "") +
                   (i > 0 ? $"#{i - 1:D3}: {aLines[i - 1]}{Environment.NewLine}" : "") +
                   $"#{i:D3}> {aLines[i]}" +
                   (i < aLines.Length - 1 ? $"{Environment.NewLine}#{i + 1:D3}: {aLines[i + 1]}" : "") +
                   (i < aLines.Length - 2 ? $"{Environment.NewLine}#{i + 2:D3}: {aLines[i + 2]}" : "");
            if (exp != null && exp.Length / 2 < act?.Length)
                for (int i = 0; i < Math.Min(exp.Length, act.Length); i++)
                    if (exp[i] != act[i])
                        Assert.AreEqual(BldLns(i, exp), BldLns(i, act), $"{Msg}: Entry{i}:");
            Assert.AreEqual(exp?.Length, act?.Length);

        }

        public static void AssertAreEqual<T>(T[] exp, T[] act, string Msg = "") where T : struct 
        {
            if (exp != null && exp.Length / 2 < act?.Length)
                for (int i = 0; i < Math.Min(exp.Length, act.Length); i++)
                    if (!EqualityComparer<T>.Default.Equals( exp[i] , act[i]))
                        Assert.AreEqual(BldLns(i, exp), BldLns(i, act), $"{Msg}: Entry{i}:");
            Assert.AreEqual(exp?.Length, act?.Length);

            static string BldLns(int i, T[] dta)
            {
                if (dta.Length < 10) return $"[{string.Join("; ", dta)}]"; 
                string sa;
                List<string> s = new() {(sa= $"{dta[i]}") };
                int size = sa.Length;
                var j = 1;
                for (; j<30; j++)
                {
                    if (i-j>=0) { s.Insert(0,sa=$"{dta[i - j]}");size += sa.Length; }
                    if (i + j < dta.Length) { s.Add(sa = $"{dta[i + j]}"); size += sa.Length; }
                    if (size > 200) break;
                }
                return $"[...{string.Join("; ", dta)}...]";
            }
        }

        public static void AssertAreEqual(string sExp, string sAct, string Msg = "")
        {
            var sSep = new String[] { Environment.NewLine };
            AssertAreEqual(sExp.Split(sSep, StringSplitOptions.None), sAct.Split(sSep, StringSplitOptions.None), Msg);
            return;
        }

}
}
