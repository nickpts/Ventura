using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ventura.Console
{
    using System;
    using System.Text;

    using Ventura;

    using Console = System.Console;

    internal class Program
    {
        static void Main(string[] args)
        {
            var gen = new Generator(Constants.CipherOption.Aes);

            string test = "All your base are belong to us";
            var bytes = Encoding.ASCII.GetBytes(test);
            
            Console.WriteLine($"Unencrypted: {test}, size: { bytes.Length }");
            var encryptedBytes = gen.GenerateRandomData(bytes);

            string encrypted = Encoding.ASCII.GetString(encryptedBytes);
            Console.WriteLine($"Encrypted: { encrypted }, size { encrypted.Length }");

            //string reconstituted = 

            Console.ReadKey();
        }
    }
}
