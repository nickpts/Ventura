//using System;
//using System.Collections.Generic;
//using System.Text;
//using Org.BouncyCastle.Crypto;
//using static Ventura.Constants;
//using Ventura.Exceptions;
//using Ventura.Interfaces;

//using Org.BouncyCastle.Crypto.Engines;
//using Org.BouncyCastle.Crypto.Parameters;
//using Org.BouncyCastle.Crypto.Modes;

//namespace Ventura.Generator
//{
//    public sealed class BouncyCastlePrng : VenturaPrng, IGenerator
//    {
//        public BouncyCastlePrng(Cipher option, byte[] seed = null) : base(option, seed)
//        {
//        }

//        protected override byte[] GenerateBlocks(int numberOfBlocks)
//        {
//            if (!state.Seeded)
//                throw new GeneratorSeedException("Generator not seeded");

//            var result = new byte[numberOfBlocks * CipherBlockSize];
//            int destArrayLength = 0;

//            var cipher = new BufferedBlockCipher(new SerpentEngine());
//            cipher.Init(true, new KeyParameter(state.Key));

//            for (int i = 0; i < numberOfBlocks; i++)
//            {
//                var plainText = state.TransformCounterToByteArray();
//                var pseudoRandomBytes = cipher.ProcessBytes(plainText, 0, plainText.Length, );

//                Array.Copy(pseudoRandomBytes, 0, result, destArrayLength, pseudoRandomBytes.Length);
//                destArrayLength += pseudoRandomBytes.Length;

//                state.Counter++;
//            }

//            return result;
//        }
//    }
//}
