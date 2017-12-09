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
    public abstract class VenturaPrng
    {
        protected Cipher option;
        internal VenturaPrngState state;

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

        /// <summary>
        /// Breaks down a byte array to 1mb blocks and encrypts each one separately
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual byte[] GenerateData(byte[] input)
        {
            var result = new byte[input.Length];
            var blocksToEncrypt = (int)Math.Ceiling((double)(input.Length / MaximumRequestSize)); // can it not return 1 if greater than 0?
            var block = new byte[MaximumRequestSize];
            int temp = 0;

            blocksToEncrypt = (blocksToEncrypt == 0) ? 1 : blocksToEncrypt; // not happy with this
            var tempArray = new byte[blocksToEncrypt * MaximumRequestSize];

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
        /// Generates up to 2^20 (1mb) worth of random data before the key is changed
        /// in order to reduce the statistical deviation from perfectly random outputs. 
        /// </summary>
        /// <param name="input">data to encrypt</param>
        /// <returns>pseudorandomly encrypted data</returns>
        protected byte[] GenerateDataPerStateKey(byte[] input)
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

        protected virtual byte[] GenerateBlocks(int numberOfBlocks)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}