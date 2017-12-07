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
            byte[] test = File.ReadAllBytes(@"C:\Users\Nick\Downloads\Programs\vlc-2.2.6-win32.exe");
           
            //string test = "All your base are belong to us";
            //var bytes = Encoding.ASCII.GetBytes(test);
            
            Console.WriteLine($"Unencrypted: {test}, size: { test.Length }");
            var encryptedBytes = gen.GenerateData(test);

            string encrypted = Encoding.ASCII.GetString(encryptedBytes);
            Console.WriteLine($"Encrypted: { encryptedBytes }, size { encryptedBytes.Length }");
            Console.WriteLine($"Arrays equal: { test.SequenceEqual(encryptedBytes) }");
            Console.ReadKey();
        }
    }
}
