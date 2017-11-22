using System;
using System.IO;
using System.Security.Cryptography;

using Medo.Security.Cryptography;

using static Ventura.Constants;

namespace Ventura
{
    public sealed class Generator
    {
        private SymmetricAlgorithm cipher;
        private byte[] key;
        

        public Generator(CipherOption option)
        {
            if (option == CipherOption.Aes)
            {
                cipher = Aes.Create(); 
            }
            else cipher = TwofishManaged.Create();

            cipher.KeySize = BlockKeySize;
        }

        public byte[] GenerateData(GeneratorState state, byte[] input)
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
                
            }

            return result;
        }
    }
}