// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Security
{

    /// <summary>
    /// An encryptor function that return an encrypted value of the argument.
    /// </summary>
    public delegate string StringEncryptor(string value);

    /// <summary>
    /// An decryptor function that return a decrypted value of the argument.
    /// </summary>
    public delegate string StringDecryptor(string value);

    /// <summary>
    /// Provides the functionality of encrypting and decrypting objects.
    /// </summary>
    public class CryptoProvider
    {
        /// <summary>
        /// The encrypting function
        /// </summary>
        public StringEncryptor Encryptor { get; set; }

        /// <summary>
        /// The decrypting function
        /// </summary>
        public StringDecryptor Decryptor { get; set; }
    }

    /// <summary>
    /// A static class contains the common encryptors
    /// </summary>
    public static class Encryptors
    {
        /// <summary>
        /// <see cref="StringEncryptor"/> that returns the same value without encryption
        /// </summary>
        public static StringEncryptor NoEncryption = value => value;
    }

    /// <summary>
    /// A static class contains the common decryptors
    /// </summary>
    public static class Decryptors
    {
        /// <summary>
        /// <see cref="StringDecryptor"/> that returns the same value without encryption
        /// </summary>
        public static StringDecryptor NoEncryption = value => value;
    }
}
