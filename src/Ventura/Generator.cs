using System;
using System.Security.Cryptography;

using Ventura.Exceptions;
using static Ventura.Constants;

using Medo.Security.Cryptography;

namespace Ventura
{
    public class Generator: IGenerator
    {
        private readonly SymmetricAlgorithm cipher;
        private readonly GeneratorState state;

        public Generator(Cipher option)
        {
            state = new GeneratorState
            {
                Counter = 0,
                Seed = new byte[] { }
            };

            switch (option)
            {
                case Cipher.Aes:
                    cipher = Aes.Create();
                    break;
                default:
                    cipher = new TwofishManaged();
                    break;
            }

            cipher.KeySize = BlockKeySize;
        }

        /// <summary>
        /// Will generate up to 2^20 worth of random data to reduce
        /// the statistical deviation from perfectly random outputs. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] GenerateRandomData(byte[] input)
        {
            if (input.Length == 0)
                throw new GeneratorInputException("Cannot encrypt empty byte array");

            if (!IsWithinAllowedSize(input))
                throw new GeneratorInputException("Cannot encrypt more than 1,048,576 bytes");

            byte[] result = new byte[input.Length];

            using (cipher)
            using (var encryptor = cipher.CreateEncryptor())
            {
                result = encryptor.TransformFinalBlock(input, 0, input.Length);

                //after every request generate an extra 256 bits of pseudorandom data 
                //and use that as the new key for the block cipher. 
            }

            cipher.Dispose();

            return result;
        }
        
        public byte[] Reseed(byte[] key)
        {
            var algorithm = SHA256.Create();
            var hash = algorithm.ComputeHash(key);

            state.Seed = hash;
            state.Counter++;
            state.Seeded = true;

            return null;
        }

        #region Private implementation

        protected string GenerateBlocks(int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded!");

            return string.Empty;
        }

        protected bool IsWithinAllowedSize(byte[] input) => input.Length <= Math.Pow(2, 20);

        #endregion
    }
}