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

        public VenturaPrng(Cipher option, byte[] seed = null)
        {
            InitializeGenerator(seed ?? Guid.NewGuid().ToByteArray());
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

            var smallestIntegral = (int)Math.Ceiling((double) (input.Length / 16));
            var pseudorandom = GenerateBlocks(input, smallestIntegral);

            return pseudorandom;
        }

        #region Private implementation

        protected void InitializeGenerator(byte[] seed)
        {
            state = new VenturaPrngState
            {
                Counter = 0,
                Key = new byte[32] // TODO: this is hardcoding
            };

            Reseed(seed);
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

        protected byte[] GenerateBlocks(byte[] sourceArray, int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded");

            var result = new byte[sourceArray.Length];

            using (cipher)
            using (var encryptor = cipher.CreateEncryptor())
            for (int i = 0; i < numberOfBlocks; i++)
            {
                var plainText = state.TransformCounterToByteArray();
                var pseudoRandomBytes = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

                // this will not work properly but will overwrite byte array every time
                Array.Copy(pseudoRandomBytes, result, pseudoRandomBytes.Length);
                state.Counter++;
            }

            return result;
        }

        #endregion
    }
}