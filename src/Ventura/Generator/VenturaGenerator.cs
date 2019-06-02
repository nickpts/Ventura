using System;
using System.Linq;
using System.Security.Cryptography;

using Ventura.Exceptions;
using Ventura.Interfaces;
using static Ventura.Constants;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace Ventura.Generator
{
    /// <summary>
    /// Converts a fixed sized state to arbitrarily long, pseudorandom outputs. 
    /// Cipher implementation is provided by BouncyCastle, currently available ciphers are
    /// Aes, Twofish, Blowfish, Serpent
    /// </summary>
    internal class VenturaGenerator: IGenerator
    {
        protected Cipher option;
        protected IBlockCipher blockCipher;
		protected IStreamCipher streamCipher;
        protected GeneratorState state;

        public VenturaGenerator(Cipher option = Cipher.Aes, byte[] seed = null)
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
        
        /// <summary>
        /// Breaks down a byte array to maximum request size blocks with a final block consisting 
        /// of the remainder. Each block is transformed with a different state key.
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GenerateData(byte[] input)
        {
            if (input.Length == 0)
                throw new GeneratorInputException("cannot encrypt empty array");

            if (input.Length <= MaximumRequestSizeForStateKey)
                return GenerateDataPerStateKey(input);

            var result = new byte[input.Length];
            var blocksToEncrypt = (int)Math.Ceiling((double)input.Length / MaximumRequestSizeForStateKey);
            var block = new byte[MaximumRequestSizeForStateKey];
            var finalBlock = new byte[input.Length % MaximumRequestSizeForStateKey];
            int temp = 0;

            do
            {
                block = GenerateDataPerStateKey(block);
                Array.Copy(block, 0, result, temp, block.Length);

                temp += block.Length;
                blocksToEncrypt--;
            }
            while (blocksToEncrypt > 1);

            finalBlock = GenerateDataPerStateKey(finalBlock);
            Array.Copy(finalBlock, 0, result, temp, finalBlock.Length);

            return result;
        }

        #region Private implementation

        protected virtual void InitialiseGenerator(byte[] seed)
        {
            state = new GeneratorState
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
					blockCipher = new AesEngine();
                    break;
                case Cipher.TwoFish:
					blockCipher = new TwofishEngine();
                    break;
                case Cipher.BlowFish:
					blockCipher = new BlowfishEngine();
                    break;
                case Cipher.Serpent:
					blockCipher = new SerpentEngine();
                    break;
				case Cipher.Isaac:
					streamCipher = new IsaacEngine();
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
            if (input.Length > MaximumRequestSizeForStateKey)
                throw new GeneratorInputException($"cannot generate array bigger than { MaximumRequestSizeForStateKey } bytes for state key");

            var roundedUpwards = (int)Math.Ceiling((double)input.Length / CipherBlockSize);
			byte[] pseudorandom = new byte[roundedUpwards];
			
			if (blockCipher != null)
			{
				pseudorandom = GenerateBlocks(roundedUpwards, new BufferedBlockCipher(blockCipher));
				Array.Clear(state.Key, 0, state.Key.Length);
				state.Key = GenerateBlocks(NumberOfBlocksForNewKey, new BufferedBlockCipher(blockCipher));

				return pseudorandom;
			}
			else if (streamCipher != null)
			{
				pseudorandom = GenerateBlocks(roundedUpwards, new BufferedStreamCipher(streamCipher));
				Array.Clear(state.Key, 0, state.Key.Length);
				state.Key = GenerateBlocks(NumberOfBlocksForNewKey, new BufferedStreamCipher(streamCipher));

				return pseudorandom;
			}

			return pseudorandom;
        }

        /// <summary>
        /// Fills each block with pseudorandom data and appends it to the result.
        /// Data used for the transformation is the counter changed into a byte array. 
        /// </summary>
        protected virtual byte[] GenerateBlocks(int numberOfBlocks, BufferedCipherBase encryptor)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded");

            var result = new byte[numberOfBlocks * CipherBlockSize];
            int destArrayLength = 0;

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