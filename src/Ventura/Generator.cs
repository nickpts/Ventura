using System;
using System.Linq;
using System.Security.Cryptography;

using Ventura.Exceptions;
using static Ventura.Constants;

using Medo.Security.Cryptography;

namespace Ventura
{
    public class Generator: IGenerator
    {
        private SymmetricAlgorithm cipher;
        private GeneratorState state;

        public Generator(Cipher option)
        {
            InitializeGenerator();
            InitializeCipher(option);
        }

        public void Reseed(byte[] seed)
        {
            var combinedSeed = state.Key.Concat(seed).ToArray();
            var hashedSeed = SHA256.Create().ComputeHash(combinedSeed);

            state.Key = hashedSeed;
            state.Counter++;
            state.Seeded = true;
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
                throw new GeneratorInputException("cannot encrypt empty array");

            if (input.Length > MaximumRequestSize)
                throw new GeneratorInputException($"cannot encrypt array bigger than { MaximumRequestSize } bytes");

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

        #region Private implementation

        protected void InitializeGenerator()
        {
            var guid = Guid.NewGuid();

            state = new GeneratorState
            {
                Counter = 0,
                Key = guid.ToByteArray()
            };
        }

        protected void InitializeCipher(Cipher option)
        {
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

        protected string GenerateBlocks(int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded!");

            var result = new byte[(16 * 1024)];

            using (cipher)
            using (var encryptor = cipher.CreateEncryptor())
            for (int i = 1; i < numberOfBlocks; i++)
            {
                
            }
            

            return string.Empty;
        }

        #endregion
    }
}