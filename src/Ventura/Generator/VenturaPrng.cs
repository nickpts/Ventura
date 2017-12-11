using System;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

using Ventura.Exceptions;
using Ventura.Interfaces;
using static Ventura.Constants;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace Ventura.Generator
{
    public class VenturaPrng: IGenerator
    {
        protected Cipher option;
        protected IBlockCipher cipher;
        protected VenturaPrngState state;

        public VenturaPrng(Cipher option = Cipher.Aes, byte[] seed = null)
        {
            if (seed == null)
                seed = Guid.NewGuid().ToByteArray();

            this.option = option;
            InitialiseGenerator(seed);
            InitialiseCipher();
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
            state.Key = GenerateBlocks(2);
        }

        /// <summary>
        /// Breaks down a byte array to maximum request size blocks and encrypts each one separately
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

        protected void InitialiseCipher()
        {
            switch (option)
            {
                case Cipher.Aes:
                    cipher = new AesEngine();
                    break;
                case Cipher.TwoFish:
                    cipher = new TwofishEngine();
                    break;
                case Cipher.BlowFish:
                    cipher= new BlowfishEngine();
                    break;
                case Cipher.Serpent:
                    cipher = new SerpentEngine();
                    break;
            }
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

            UpdateKey();

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

            var encryptor = new BufferedBlockCipher(cipher);
            encryptor.Init(true, new KeyParameter(state.Key));

            for (int i = 0; i < numberOfBlocks; i++)
            {
                var plainText = state.TransformCounterToByteArray();
                encryptor.ProcessBytes(plainText, 0, plainText.Length, result, destArrayLength);

                destArrayLength += plainText.Length;
                state.Counter++;
            }

            return result;
        }

        #endregion
    }
}