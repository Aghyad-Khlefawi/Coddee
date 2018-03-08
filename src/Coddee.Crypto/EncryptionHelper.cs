// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Coddee.Crypto
{
    /// <summary>
    /// Helper class for encryption operations.
    /// </summary>
    public class EncryptionHelper
    {
        /// <summary>
        /// Encrypts a string using symmetric algorithm
        /// </summary>
        /// <param name="text">The text to encrypt</param>
        /// <param name="secrectKey">The encryption key</param>
        /// <param name="alg">The encryption algorithm, If kept null then the AesManaged algorithm will be used</param>
        public static byte[] EncryptString(string text,
                                           string secrectKey,
                                           SymmetricAlgorithm alg=null)
        {
            using (alg = alg ?? new AesManaged())
            {
                try
                {
                    alg.Key =
                        new Rfc2898DeriveBytes(secrectKey, Encoding.ASCII.GetBytes(secrectKey))
                            .GetBytes(alg.KeySize / 8);
                    alg.IV =
                        new Rfc2898DeriveBytes(secrectKey, Encoding.ASCII.GetBytes(secrectKey))
                            .GetBytes(alg.BlockSize / 8);
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException("The secret key must be at least 8 bytes long", e);
                }
                ICryptoTransform encryptor = alg.CreateEncryptor(alg.Key, alg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt =
                        new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Encrypts a string using AesManaged algorithm
        /// </summary>
        /// <param name="text">The text to encrypt</param>
        /// <param name="secrectKey">The encryption key</param>
        /// <param name="alg">The encryption algorithm, If kept null then the AesManaged algorithm will be used</param>
        public static string EncryptStringAsBase64(string text,
                                                   string secrectKey,
                                                   SymmetricAlgorithm alg = null)
        {
            
                return Convert.ToBase64String(EncryptString(text, secrectKey, alg));
        }

        /// <summary>
        /// Decrypt bytes using AesManaged algorithm
        /// </summary>
        /// <param name="enc">The encrypted bytes</param>
        /// <param name="secrectKey">The encryption key</param>
        /// <param name="alg">The encryption algorithm, If kept null then the AesManaged algorithm will be used</param>
        public static string Decrypt(byte[] enc,
                                     string secrectKey,
                                     SymmetricAlgorithm alg=null)
        {
            using (alg = alg ?? new AesManaged())
            {
                try
                {
                    alg.Key =
                        new Rfc2898DeriveBytes(secrectKey, Encoding.ASCII.GetBytes(secrectKey))
                            .GetBytes(alg.KeySize / 8);
                    alg.IV =
                        new Rfc2898DeriveBytes(secrectKey, Encoding.ASCII.GetBytes(secrectKey))
                            .GetBytes(alg.BlockSize / 8);
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException("The secret key must be at least 8 bytes long", e);
                }
                ICryptoTransform decryptor = alg.CreateDecryptor(alg.Key, alg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(enc))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt a Base64 string using AesManaged algorithm
        /// </summary>
        /// <param name="text">The encrypted text</param>
        /// <param name="secrectKey">The encryption key</param>
        /// <param name="alg">The encryption algorithm, If kept null then the AesManaged algorithm will be used</param>
        public static string Decrypt(string text,
                                     string secrectKey,
                                     SymmetricAlgorithm alg = null)
        {
            
                return Decrypt(Convert.FromBase64String(text), secrectKey, alg);
        }
    }
}