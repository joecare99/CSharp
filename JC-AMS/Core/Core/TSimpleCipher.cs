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
using JCAMS.Core.Logging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace JCAMS.Core
{
    /// <summary>
    /// Class TSimpleCipher provides a simple cipher.
    /// </summary>
    public static class TSimpleCipher
    {
        #region Properties
        /// <summary>
        /// The encryption key
        /// </summary>
        private static byte[] EncryptionKey= new byte[24]
            { // Really ? !!! "Please change the lock on my suitcase ..."
                1,  2,	3,	4,	5,	6,	7,	8,
                9,  10,	11,	12,	13,	14,	15,	16,
                17, 18,	19,	20,	21,	22,	23,	24
            };

    /// <summary>
    /// The encryptio initialization vector
    /// </summary>
    private static byte[] EncryptioInitializationVector = new byte[8]
            {
                65, 110,    68, 26, 69, 178,    200,    219
            };
        #endregion

        #region Methods
        /// <summary>
        /// Initializes static members of the <see cref="TSimpleCipher"/> class.
        /// </summary>
        static TSimpleCipher()
        { }

        /// <summary>Does a simple the decrypt.</summary>
        /// <param name="sCypher">The crypted text.</param>
        /// <returns>System.String.</returns>
        public static string SimpleDecrypt(string sCypher)
        {
            if (string.IsNullOrEmpty(sCypher) || sCypher.Length % 4 > 0) return "";
            try
            {
                byte[] result;
                byte[] inputInBytes = Convert.FromBase64String(sCypher);
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms,
                     TripleDES.Create().CreateDecryptor(EncryptionKey, EncryptioInitializationVector),
                    CryptoStreamMode.Write))
                {
                    cs.Write(inputInBytes, 0, inputInBytes.Length);
                    cs.FlushFinalBlock();
                    ms.Position = 0L;
                    result = new byte[ms.Length];
                    ms.Read(result, 0, (int)ms.Length);
                    cs.Close();
                }
                return new UTF8Encoding().GetString(result);
            }
            catch (Exception ex)
            {
                TLogging.Log(ex);
                return "";
            }
        }

        /// <summary>
        /// Does a simple encrypt.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns>System.String.</returns>
        public static string SimpleEncrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return "";          
            try
            {
                byte[] result;
                byte[] inputInBytes = new UTF8Encoding().GetBytes(plainText);
                using (var encryptedStream = new MemoryStream())

                using (var cryptStream = new CryptoStream(encryptedStream,
                TripleDES.Create().CreateEncryptor(EncryptionKey, EncryptioInitializationVector),
                CryptoStreamMode.Write))
                {
                    cryptStream.Write(inputInBytes, 0, inputInBytes.Length);
                    cryptStream.FlushFinalBlock();
                    encryptedStream.Position = 0L;
                    result = new byte[encryptedStream.Length];
                    encryptedStream.Read(result, 0, (int)encryptedStream.Length);
                    cryptStream.Close();
                }
                return Convert.ToBase64String(result);
            }
            catch (Exception ex)
            {
                TLogging.Log(ex);
                return "";
            }
        }
        #endregion
    }
}