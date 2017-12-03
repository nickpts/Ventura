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
            if (seed == null)
                seed = Guid.NewGuid().ToByteArray();

            InitializeGenerator(seed);
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

            var roundedUpwards = (int)Math.Ceiling((double) input.Length / CipherBlockSize); 
            var pseudorandom = GenerateBlocks(input, roundedUpwards);

            return pseudorandom;
        }

        #region Private implementation

        protected void InitializeGenerator(byte[] seed)
        {
            state = new VenturaPrngState
            {
                Key = new byte[KeyBlockSize] 
            };

            Reseed(seed);
        }

        protected void InitializeCipher(Cipher option)
        {
            switch (option)
            {
                case Cipher.Aes:
                    cipher = Rijndael.Create();
                    break;
                default:
                    cipher = new TwofishManaged();
                    break;
            }

            cipher.Mode = CipherMode.ECB;
            cipher.Padding = PaddingMode.None;
        }

        protected byte[] GenerateBlocks(byte[] sourceArray, int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded");

            var result = new byte[sourceArray.Length];
            var tempArray = new byte[numberOfBlocks * CipherBlockSize]; 

            int destArrayLenth = 0;

            using (cipher)
            for (int i = 0; i < numberOfBlocks; i++)
            using (var encryptor = cipher.CreateEncryptor())
            {
                var plainText = state.TransformCounterToByteArray();
                cipher.IV = plainText;

                var pseudoRandomBytes = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
                
                Array.Copy(pseudoRandomBytes, 0, tempArray, destArrayLenth, pseudoRandomBytes.Length);
                destArrayLenth = pseudoRandomBytes.Length;

                state.Counter++;
            }

            Array.Resize(ref tempArray, sourceArray.Length);
            Array.Copy(tempArray, result, tempArray.Length);

            return result;
        }

        #endregion
    }
}