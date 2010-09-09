/***********************************************************************\
 * Virtuoso.Miranda.Plugins (Hyphen)                                   *
 * Provides a managed wrapper for API of IM client Miranda.            *
 * Copyright (C) 2006-2009 virtuoso                                    *
 *                    deml.tomas@seznam.cz                             *
 *                                                                     *
 * This library is free software; you can redistribute it and/or       *
 * modify it under the terms of the GNU Lesser General Public          *
 * License as published by the Free Software Foundation; either        *
 * version 2.1 of the License, or (at your option) any later version.  *
 *                                                                     *
 * This library is distributed in the hope that it will be useful,     *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of      *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU   *
 * Lesser General Public License for more details.                     *
\***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Virtuoso.Miranda.Plugins.Configuration
{
    public abstract class PortableEncryption : IEncryption
    {
        #region Fields

        private static readonly byte[] KeyGeneratorSalt = new byte[] { 13, 74, 64, 0, 11, 128, 32, 44, 113, 42 };

        #endregion

        #region .ctors

        protected PortableEncryption() { }

        #endregion

        #region Methods

        public virtual byte[] Encrypt(byte[] data)
        {
            ICryptoTransform transform = CreateEncryptor();

            using (MemoryStream stream = new MemoryStream(data.Length))
            {
                using (CryptoStream crypto = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                    crypto.Write(data, 0, data.Length);

                return stream.ToArray();
            }
        }

        public virtual byte[] Decrypt(byte[] data)
        {
            ICryptoTransform transform = CreateDecryptor();

            using (MemoryStream inStream = new MemoryStream(data), outStream = new MemoryStream(data.Length))
            {
                using (CryptoStream crypto = new CryptoStream(inStream, transform, CryptoStreamMode.Read))
                {
                    int count = 0;
                    byte[] buffer = new byte[2048];

                    while ((count = crypto.Read(buffer, 0, buffer.Length)) != 0)
                        outStream.Write(buffer, 0, count);                    
                }

                return outStream.ToArray();
            }
        }

        #endregion		

        #region Encryption

        protected virtual ICryptoTransform CreateEncryptor()
        {
            string key = PromptForKey(false);

            TripleDES tripleDes = TripleDES.Create();
            byte[] keyBytes;
            byte[] ivBytes;
            GetSecretBytes(tripleDes, key, out keyBytes, out ivBytes);

            return tripleDes.CreateEncryptor(keyBytes, ivBytes);
        }

        protected virtual ICryptoTransform CreateDecryptor()
        {
            string key = PromptForKey(true);

            TripleDES tripleDes = TripleDES.Create();
            byte[] keyBytes;
            byte[] ivBytes;
            GetSecretBytes(tripleDes, key, out keyBytes, out ivBytes);

            return tripleDes.CreateDecryptor(keyBytes, ivBytes);
        }

        protected static void GetSecretBytes(SymmetricAlgorithm algorithm, string password, out byte[] keyBytes, out byte[] ivBytes)
        {
            Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(password, KeyGeneratorSalt, 20);
            keyBytes = keyGenerator.GetBytes(algorithm.LegalKeySizes[0].MaxSize / 8);

            keyGenerator.IterationCount = 10;
            ivBytes = keyGenerator.GetBytes(algorithm.LegalBlockSizes[0].MaxSize / 8);
        }

        #endregion		

        #region Abstracts

        protected abstract string PromptForKey(bool decrypting);

        #endregion
    }
}
 