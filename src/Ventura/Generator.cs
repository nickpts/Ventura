using System;
using System.IO;
using System.Resources;
using System.Security.Cryptography;

using Medo.Security.Cryptography;
using Ventura.Exceptions;
using static Ventura.Constants;

namespace Ventura
{
    public sealed class Generator: IGenerator
    {
        private SymmetricAlgorithm cipher;
        private GeneratorState state;

        public Generator(CipherOption option)
        {
            state = new GeneratorState
            {
                Counter = 0,
                Seed = new byte[] { }
            };

            switch (option)
            {
                case CipherOption.Aes:
                    cipher = Aes.Create();
                    break;
                default:
                    cipher = TwofishManaged.Create();
                    break;
            }

            cipher.KeySize = 256;
        }

        public byte[] GenerateRandomData(byte[] input)
        {
            byte[] result = new byte[input.Length];

            using (cipher)
            using (var encryptor = cipher.CreateEncryptor())
            using (var memStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
            {
                // cryptoStream.Write(result, 0, 0);
                cryptoStream.Write(input, 0, input.Length);
                Array.Copy(input, result, input.Length);


                //after every request generate an extra 256 bits of pseudorandom data 
                //and use that as the new key for the block cipher. 
            }
            cipher.Dispose();

            return result;
        }

        private string GenerateBlocks(GeneratorState state, int numberOfBlocks)
        {
            if (!state.Seeded)
                throw new GeneratorSeedException("Generator not seeded!");

            return string.Empty;
        }

        public byte[] Reseed(GeneratorState state, byte[] key)
        {
            var algorithm = SHA256.Create();
            var hash = algorithm.ComputeHash(key);

            state.Seed = hash;
            state.Counter++;
            state.Seeded = true;

            return null;
        }
    }
}