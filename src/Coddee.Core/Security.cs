// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Security
{
    public delegate string StringEncryptor(string value);
    public delegate string StringDecryptor(string value);

    public class CryptoProvider
    {
        public StringEncryptor Encryptor { get; set; }
        public StringDecryptor Decryptor { get; set; }
    }

    public class Encryptors
    {
        public static StringEncryptor NoEncryption = value => value;
    }
    public class Decryptors
    {
        public static StringDecryptor NoEncryption = value => value;
    }
}
