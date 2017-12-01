using System;
using System.Linq;
using System.Security.Cryptography;

using Ventura.Exceptions;
using Ventura.Interfaces;
using static Ventura.Constants;

using Medo.Security.Cryptography;

namespace Ventura.Generator
{
    public class VenturaPrng: IGenerator
    {
        private SymmetricAlgorithm cipher;
        private VenturaPrngState state;

        public VenturaPrng(Cipher option)
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
        /// <param name="input">data to encrypt</param>
        /// <returns>pseudorandomly encrypted data</returns>
        public byte[] GenerateRandomData(byte[] input)
        {
            if (input.Length == 0)
                throw new GeneratorInputException("cannot encrypt empty array");

            if (input.Length > MaximumRequestSize)
                throw new GeneratorInputException($"cannot encrypt array bigger than { MaximumRequestSize } bytes");

            byte[] result = new byte[input.Length];

            return result;
        }

        #region Private implementation

        protected void InitializeGenerator()
        {
            state = new VenturaPrngState
            {
                Counter = 0,
                Key = Guid.NewGuid().ToByteArray()
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

            cipher.Mode = CipherMode.ECB;
            cipher.Padding = PaddingMode.None;
            cipher.KeySize = BlockKeySize;
        }

        protected byte[] GenerateBlocks(int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded");

            var result = new byte[(16 * 1024)];

            using (cipher)
            using (var encryptor = cipher.CreateEncryptor())
            for (int i = 1; i < numberOfBlocks; i++)
            {
                
            }


            return result;
        }

        #endregion
    }
}