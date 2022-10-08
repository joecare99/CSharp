using System;
using System.Data.SqlClient;
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
    public static class TExceptionHelper
    {

        /// <summary>
        /// The m last exception
        /// </summary>
        public static Exception m_LastException;

        /// <summary>
        /// ses the exception.
        /// </summary>
        /// <param name="Ex">The ex.</param>
        /// <returns>System.String.</returns>
        public static string AsString(this Exception Ex)
        {
            StringBuilder sText = new StringBuilder();
            sText.AppendFormat("Exception;{0};{1};{2}", Ex.Message.ToString(), Ex.Source?.ToString(), Ex.TargetSite?.ToString());
            try
            {
                if (Ex.InnerException != null)
                {
                    sText.AppendFormat("\n   InnerException;{0}", Ex.InnerException.Message.ToString());
                }
                if (Ex.StackTrace != null)
                {
                    sText.AppendFormat("\n   StackTrace;{0}", Ex.StackTrace.ToString());
                }
            }
            catch
            {
            }
            return sText.ToString();
        }

        /// <summary>
        /// ses the exception.
        /// </summary>
        /// <param name="Ex">The ex.</param>
        /// <returns>System.String.</returns>
        public static string AsString(this SqlException Ex)
        {
            StringBuilder SB = new StringBuilder();
            SB.AppendFormat("Exception;{0};{1};{2}", Ex.Message.ToString(), Ex.Source.ToString(), Ex.TargetSite.ToString());
            try
            {
                for (int I = 0; I < Ex.Errors.Count; I++)
                {
                    SB.AppendFormat("    Index # {0}", I);
                    SB.AppendFormat("   Message: {0}", Ex.Errors[I].Message);
                    SB.AppendFormat("LineNumber: {0}", Ex.Errors[I].LineNumber);
                    SB.AppendFormat("    Source: {0}", Ex.Errors[I].Source);
                    SB.AppendFormat(" Procedure: {0}", Ex.Errors[I].Procedure);
                }
                if (Ex.InnerException != null)
                {
                    SB.AppendFormat("\n   InnerException;{0}", Ex.InnerException.Message.ToString());
                }
                if (Ex.StackTrace != null)
                {
                    SB.AppendFormat("\n   StackTrace;{0}", Ex.StackTrace.ToString());
                }
            }
            catch
            {
            }
            return SB.ToString();
        }
    }
}