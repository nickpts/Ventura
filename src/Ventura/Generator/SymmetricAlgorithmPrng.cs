using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using static Ventura.Constants;
using Ventura.Exceptions;
using Ventura.Interfaces;

namespace Ventura.Generator
{
    public sealed class SymmetricAlgorithmPrng : VenturaPrng, IGenerator
    {
        public SymmetricAlgorithmPrng(byte[] seed = null) : base(Cipher.Aes, seed)
        {

        }

        /// <summary>
        /// Fills each block with pseudorandom data and appends it to the result.
        /// Data used for the transformation is the counter changed into a byte array. 
        /// </summary>
        protected override byte[] GenerateBlocks(int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded");

            using (var cipher = new RijndaelManaged())
            {
                cipher.Mode = CipherMode.ECB;
                cipher.Padding = PaddingMode.None;
                cipher.Key = state.Key;

                var result = new byte[numberOfBlocks * CipherBlockSize];

                int destArrayLength = 0;

                for (int i = 0; i < numberOfBlocks; i++)
                    using (var encryptor = cipher.CreateEncryptor())
                    {
                        var plainText = state.TransformCounterToByteArray();
                        var pseudoRandomBytes = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

                        Array.Copy(pseudoRandomBytes, 0, result, destArrayLength, pseudoRandomBytes.Length);
                        destArrayLength += pseudoRandomBytes.Length;

                        state.Counter++;
                    }

                return result;
            }
        }
    }
}
