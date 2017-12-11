using System;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Ventura.Exceptions;
using Ventura.Interfaces;
using static Ventura.Constants;

namespace Ventura.Generator
{
    public class VenturaPrng
    {
        protected Cipher option;
        protected VenturaPrngState state;

        public VenturaPrng(Cipher option = Cipher.Aes, byte[] seed = null)
        {
            if (seed == null)
                seed = Guid.NewGuid().ToByteArray();

            this.option = option;
            InitialiseGenerator(seed);
        }

        public void Reseed(byte[] seed)
        {
            var combinedSeed = state.Key.Concat(seed).ToArray();
            var hashedSeed = SHA256.Create().ComputeHash(combinedSeed);

            state.Key = hashedSeed;
            state.Counter++;
            state.Seeded = true;
        }

        public void UpdateKey()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Breaks down a byte array to 1mb blocks and encrypts each one separately
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual byte[] GenerateData(byte[] input)
        {
            if (input.Length == 0)
                throw new GeneratorInputException("cannot encrypt empty array");

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

        protected virtual void InitialiseGenerator(byte[] seed)
        {
            state = new VenturaPrngState
            {
                Key = new byte[KeyBlockSize]
            };

            Reseed(seed);
        }

        /// <summary>
        /// Generates up to 2^20 (1mb) worth of random data and then changes the key
        /// in order to reduce the statistical deviation from perfectly random outputs. 
        /// </summary>
        /// <param name="input">data to encrypt</param>
        /// <returns>pseudorandomly encrypted data</returns>
        protected virtual byte[] GenerateDataPerStateKey(byte[] input)
        {
            if (input.Length > MaximumRequestSize)
                throw new GeneratorInputException($"cannot generate array bigger than { MaximumRequestSize } bytes for state key");

            var roundedUpwards = (int)Math.Ceiling((double)input.Length / CipherBlockSize);
            var pseudorandom = GenerateBlocks(roundedUpwards);

            // TODO: add a test to check this is called, check if array needs to be resized?
            state.Key = GenerateBlocks(2);

            return pseudorandom;
        }

        /// <summary>
        /// Fills each block with pseudorandom data and appends it to the result.
        /// Data used for the transformation is the counter changed into a byte array. 
        /// </summary>
        protected virtual byte[] GenerateBlocks(int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded");

            var result = new byte[numberOfBlocks * CipherBlockSize];
            int destArrayLength = 0;

            var cipher = new BufferedBlockCipher(new AesEngine());
            cipher.Init(true, new KeyParameter(state.Key));

            for (int i = 0; i < numberOfBlocks; i++)
            {
                var plainText = state.TransformCounterToByteArray();
                cipher.ProcessBytes(plainText, 0, plainText.Length, result, destArrayLength);

                destArrayLength += plainText.Length;
                state.Counter++;
            }

            return result;
        }

        

        #endregion
    }
}