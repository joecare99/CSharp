// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="T.cs" company="JC-Soft">
//     Copyright © JC-Soft GmbH 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using JCAMS.Core.Logging;
using Microsoft.Win32;
using System;
using System.Globalization;
namespace JCAMS.Core
{
    public static class SDotNetUtil
    {

        /// <summary>
        /// Checks the dot net version.
        /// </summary>
        /// <param name="theFramework">The framework.</param>
        /// <param name="theServicePack">The service pack.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckDotNETVersion(double theFramework, int theServicePack)
        {
            double Framework = 0.0;
            int ServicePack = 0;
            string[] version_names = GetDotNETVersions(out RegistryKey installed_versions);
            if (version_names == null || version_names.Length < 1) { return false; }
            for (int I = 1; I < version_names.Length; I++)
            {
                Framework = Convert.ToDouble(version_names[I].Substring(1, Math.Min(3, version_names[I].Length-1)), CultureInfo.InvariantCulture);
                ServicePack = Convert.ToInt32(installed_versions.OpenSubKey(version_names[I]).GetValue("SP", 0));
                if (Framework == theFramework && ServicePack == theServicePack)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the dot net version.
        /// </summary>
        /// <param name="Framework">The framework.</param>
        /// <param name="Servicepack">The servicepack.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetDotNETVersion(out double Framework, out int Servicepack)
        {
            try
            {
                string[] version_names = GetDotNETVersions(out RegistryKey installed_versions);
                if (version_names == null || version_names.Length < 1) { (Framework, Servicepack) = (0d, 0); return false; }
                Framework = Convert.ToDouble(version_names[version_names.Length - 1].Substring(1, 3), CultureInfo.InvariantCulture);
                Servicepack = Convert.ToInt32(installed_versions.OpenSubKey(version_names[version_names.Length - 1]).GetValue("SP", 0));
                return true;
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
                (Framework, Servicepack) = (0d, 0);
                return false;
            }
        }

        private static string[] GetDotNETVersions(out RegistryKey installed_versions)
        {
            installed_versions = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP");
            if (installed_versions == null) { return null; }
            return installed_versions.GetSubKeyNames();

        }
    }
}