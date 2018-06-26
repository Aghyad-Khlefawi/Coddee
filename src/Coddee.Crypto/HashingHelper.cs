// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Security.Cryptography;
using System.Text;

namespace Coddee.Crypto
{
    /// <summary>
    /// Represent a hash result
    /// </summary>
    public class HashedValue
    {
        /// <inheritdoc />
        public HashedValue(string hash,
                           string salt)
        {
            Hash = hash;
            Salt = salt;
        }

        /// <summary>
        /// The hash value.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// The salt used for hashing.
        /// </summary>
        public string Salt { get; set; }
    }

    /// <summary>
    /// Helper class for hashing operations.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// hashes a text value
        /// </summary>
        /// <param name="text">The text to hash</param>
        /// <param name="salt">salt to use</param>
        /// <param name="alg">The hashing algorithm, if kept null the SHA256 will be used</param>
        /// <returns></returns>
        public static HashedValue GenerateHash(string text,
                                               string salt,
                                               HashAlgorithm alg = null)
        {
            using (alg = alg ?? SHA256.Create())
            {
                byte[] textbytes = Encoding.ASCII.GetBytes(text);
                byte[] saltbytes = Encoding.ASCII.GetBytes(salt);
                byte[] finalresult = new byte[textbytes.Length + saltbytes.Length];
                Buffer.BlockCopy(textbytes, 0, finalresult, 0, textbytes.Length);
                Buffer.BlockCopy(saltbytes, 0, finalresult, textbytes.Length, saltbytes.Length);
                byte[] hashData = alg.ComputeHash(finalresult);
                alg.Dispose();
                return new HashedValue(Encoding.ASCII.GetString(hashData), salt);
            }
        }

        /// <summary>
        /// hashes a text value
        /// </summary>
        /// <param name="text">The text to hash with a random salt</param>
        /// <param name="alg">The hashing algorithm, if kept null the SHA256 will be used</param>
        /// <returns></returns>
        public static HashedValue GenerateHash(string text,
                                               HashAlgorithm alg = null)
        {
            return GenerateHash(text, GenerateRandomSalt(RandomNumberGenerator.Create(), 16), alg);
        }

        /// <summary>
        /// Compares a hash to a plain text by hashing the plain text an comparing the hashes
        /// </summary>
        /// <param name="text">The compared value</param>
        /// <param name="salt">The salt used for the hash</param>
        /// <param name="hash">The original hash</param>
        /// <returns></returns>
        public static bool ValidateHash(string text, string salt, string hash)
        {
            return GenerateHash(text, salt).Hash == hash;
        }

        /// <summary>
        /// Generate a random salt for the hashing
        /// </summary>
        /// <param name="rng">CryptoServiceProvider</param>
        /// <param name="size">salt size</param>
        /// <returns></returns>
        public static string GenerateRandomSalt(RandomNumberGenerator rng, int size)
        {
            var bytes = new Byte[size];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Validate a hash in Base64 format
        /// </summary>
        /// <param name="text">The compared value</param>
        /// <param name="salt">The salt used for the hash</param>
        /// <param name="hash">The original hash</param>
        public static bool ValidateHashString64(string text, string salt, string hash)
        {
            var temp = GenerateHash(text, salt).Hash;
            byte[] textbytes = Encoding.ASCII.GetBytes(temp);
            var string64 = Convert.ToBase64String(textbytes);
            return string64 == hash;
        }
    }
}