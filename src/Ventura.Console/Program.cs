using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Ventura.Generator;

namespace Ventura.Console
{
    using System;
    using System.Linq;
    using System.Text;

    using Ventura;

    using Console = System.Console;

    internal class Program  
    {
        static void Main(string[] args)
        {
            var gen = new VenturaPrng(Constants.Cipher.Aes);

            //Stream stream;
            byte[] input = File.ReadAllBytes(@"C:\Users\Nick\Downloads\Programs\JetBrains.ReSharperUltimate.2017.2.2.exe");
           
            string test = "All your base are belong to us";
            var bytes = Encoding.ASCII.GetBytes(test);
            
            Console.WriteLine($"Unencrypted: {input}, size: { input.Length }");
            var encryptedBytes = gen.GenerateData(input);

            //string encrypted = Encoding.ASCII.GetString(encryptedBytes);
            Console.WriteLine($"Encrypted: { encryptedBytes }, size { encryptedBytes.Length }");
            Console.WriteLine($"Arrays equal: { bytes.SequenceEqual(encryptedBytes) }");
            Console.ReadKey();
        }
    }
}
