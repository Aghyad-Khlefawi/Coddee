// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Text;

namespace Coddee.Crypto
{
    /// <summary>
    /// Represent a hashed password
    /// </summary>
    public class HashedPassword
    {
        /// <inheritdoc />
        public HashedPassword(string password, string salt)
        {
            Password = password;
            Salt = salt;
        }

        /// <summary>
        /// The hashed password value.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Hash salt.
        /// </summary>
        public string Salt { get; set; }
    }

    /// <summary>
    /// Helper class for password hashing and generating.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Hashes a string and return a result formated in Base64 
        /// </summary>
        /// <param name="password">password to hash</param>
        /// <returns></returns>
        public static HashedPassword GenerateHashedPassword(string password)
        {
            var temp = HashHelper.GenerateHash(password);
            byte[] textbytes = Encoding.ASCII.GetBytes(temp.Hash);
            var string64 = Convert.ToBase64String(textbytes);
            return new HashedPassword(string64, temp.Salt);
        }

        /// <summary>
        /// Validate a Base64 password value
        /// </summary>
        /// <param name="password">The compared value</param>
        /// <param name="salt">The salt used for the hash</param>
        /// <param name="hash">The original hash</param>
        /// <returns></returns>
        public static bool ValidatePassword(string password, string salt, string hash)
        {
            return HashHelper.ValidateHashString64(password, salt, hash);
        }
    }

}
