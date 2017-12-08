using System;
using System.Linq;
using System.Security.Cryptography;

using Ventura.Exceptions;
using Ventura.Interfaces;
using static Ventura.Constants;

using Medo.Security.Cryptography;
using BlowFishCS;

/// <summary>
/// 
/// </summary>
namespace Ventura.Generator
{
    public sealed class VenturaPrng : IGenerator
    {
        private Cipher option;
        private VenturaPrngState state;

        public VenturaPrng(Cipher option, byte[] seed = null)
        {
            if (seed == null)
                seed = Guid.NewGuid().ToByteArray();

            this.option = option;
            InitializeGenerator(seed);
        }

        public void Reseed(byte[] seed)
        {
            var combinedSeed = state.Key.Concat(seed).ToArray();
            var hashedSeed = SHA256.Create().ComputeHash(combinedSeed);

            state.Key = hashedSeed;
            state.Counter++;
            state.Seeded = true;
        }

        public byte[] GenerateData(byte[] input)
        {
            if (input.Length == 0)
                throw new GeneratorInputException("cannot encrypt empty array");

            var result = new byte[input.Length];

            if (option == Cipher.Aes)
            {
                using (var rijndael = Rijndael.Create())
                {
                    result = GenerateDataWithSymmetricAlgorithm(rijndael, input);
                }
            }
            else if (option == Cipher.TwoFish)
            {
                using (var twoFish = new TwofishManaged())
                {
                    result = GenerateDataWithSymmetricAlgorithm(twoFish, input);
                }
            }
            else if (option == Cipher.BlowFish)
            {

                result = GenerateDataWithBlowFish(input);
            }

            return result;
        }

        #region Private implementation

        private void InitializeGenerator(byte[] seed)
        {
            state = new VenturaPrngState
            {
                Key = new byte[KeyBlockSize]
            };

            Reseed(seed);
        }

        /// <summary>
        /// Breaks down a byte array to 1mb blocks and encrypts each one separately
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] GenerateDataWithSymmetricAlgorithm(SymmetricAlgorithm cipher, byte[] input)
        {
            cipher.Mode = CipherMode.ECB;
            cipher.Padding = PaddingMode.None;
            cipher.Key = state.Key;

            var result = new byte[input.Length];
            var blocksToEncrypt = (int)Math.Ceiling((double)(input.Length / MaximumRequestSize)); // can it not return 1 if greater than 0?
            var tempArray = new byte[blocksToEncrypt * MaximumRequestSize];
            var block = new byte[MaximumRequestSize];
            int temp = 0;

            blocksToEncrypt = (blocksToEncrypt == 0) ? 1 : blocksToEncrypt; // not happy with this

            do
            {
                block = GenerateDataPerStateKey(cipher, block);
                Array.Copy(block, 0, tempArray, temp, block.Length);

                temp += block.Length;
                blocksToEncrypt--;
            }
            while (blocksToEncrypt > 0);

            Array.Resize(ref tempArray, input.Length);
            Array.Copy(tempArray, result, tempArray.Length);

            return result;
        }

        private byte[] GenerateDataWithBlowFish(byte[] input)
        {
            var result = new byte[input.Length];
            var blocksToEncrypt = (int)Math.Ceiling((double)(input.Length / MaximumRequestSize)); // can it not return 1 if greater than 0?
            var tempArray = new byte[blocksToEncrypt * MaximumRequestSize];
            var block = new byte[MaximumRequestSize];
            int temp = 0;

            blocksToEncrypt = (blocksToEncrypt == 0) ? 1 : blocksToEncrypt; // not happy with this

            do
            {
                block = GenerateDataPerStateKey(block);
                Array.Copy(block, 0, tempArray, temp, block.Length);

                temp += block.Length;
                blocksToEncrypt--;
            }
            while (blocksToEncrypt > 0);

            Array.Resize(ref tempArray, input.Length);
            Array.Copy(tempArray, result, tempArray.Length);

            return result;
        }

        private byte[] GenerateDataPerStateKey(byte[] input)
        {
            if (input.Length == 0)
                throw new GeneratorInputException("cannot encrypt empty array");

            if (input.Length > MaximumRequestSize)
                throw new GeneratorInputException($"cannot generate array bigger than { MaximumRequestSize } bytes");

            var roundedUpwards = (int)Math.Ceiling((double)input.Length / CipherBlockSize);
            var pseudorandom = GenerateBlocks(roundedUpwards);

            // TODO: add a test to check this is called, check if array needs to be resized?
            state.Key = GenerateBlocks(2);

            return pseudorandom;
        }

        private byte[] GenerateBlocks(int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded");

            var result = new byte[numberOfBlocks * CipherBlockSize];

            int destArrayLength = 0;

            for (int i = 0; i < numberOfBlocks; i++)
            {
                var plainText = state.TransformCounterToByteArray();
                var cipher = new BlowFish(state.Key);
                cipher.SetRandomIV();

                var pseudoRandomBytes = cipher.Encrypt_CBC(plainText);

                Array.Copy(pseudoRandomBytes, 0, result, destArrayLength, pseudoRandomBytes.Length);
                destArrayLength += pseudoRandomBytes.Length;

                state.Counter++;
            }

            return result;
        }

        /// <summary>
        /// Will generate up to 2^20 (1mb) worth of random data before the key is changed
        /// in order to reduce the statistical deviation from perfectly random outputs. 
        /// </summary>
        /// <param name="input">data to encrypt</param>
        /// <returns>pseudorandomly encrypted data</returns>
        private byte[] GenerateDataPerStateKey(SymmetricAlgorithm cipher, byte[] input)
        {
            if (input.Length == 0)
                throw new GeneratorInputException("cannot encrypt empty array");

            if (input.Length > MaximumRequestSize)
                throw new GeneratorInputException($"cannot generate array bigger than { MaximumRequestSize } bytes");

            var roundedUpwards = (int)Math.Ceiling((double)input.Length / CipherBlockSize);
            var pseudorandom = GenerateBlocks(cipher, roundedUpwards);

            // TODO: add a test to check this is called, check if array needs to be resized?
            state.Key = GenerateBlocks(cipher, 2);

            return pseudorandom;
        }

        /// <summary>
        /// Fills each block with pseudorandom data and appends it to the result.
        /// Data used for the transformation is the counter changed into a byte array. 
        /// </summary>
        private byte[] GenerateBlocks(SymmetricAlgorithm cipher, int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded");

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

        #endregion
    }
}