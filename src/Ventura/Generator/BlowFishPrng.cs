using System;
using System.Collections.Generic;
using System.Text;

using static Ventura.Constants;
using Ventura.Exceptions;
using Ventura.Interfaces;

using BlowFishCS;

namespace Ventura.Generator
{
    public sealed class BlowFishPrng: VenturaPrng, IGenerator
    {
        public BlowFishPrng(byte[] seed = null)
            :base(Cipher.BlowFish, seed)
        {

        }

        public override byte[] GenerateData(byte[] input)
        {
            return base.GenerateData(input);
        }

        protected override byte[] GenerateBlocks(int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded");

            var result = new byte[numberOfBlocks * CipherBlockSize];
            int destArrayLength = 0;

            var cipher = new BlowFish(state.Key);
            cipher.SetRandomIV();

            for (int i = 0; i < numberOfBlocks; i++)
            {
                var plainText = state.TransformCounterToByteArray();
                var pseudoRandomBytes = cipher.Encrypt_CBC(plainText);

                Array.Copy(pseudoRandomBytes, 0, result, destArrayLength, pseudoRandomBytes.Length);
                destArrayLength += pseudoRandomBytes.Length;

                state.Counter++;
            }

            return result;
        }
    }
}
