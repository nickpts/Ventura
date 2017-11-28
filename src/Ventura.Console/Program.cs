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
            
            Console.WriteLine($"Input: { GetApproximateSize(bytes)}");

            //foreach (var encryptedByte in gen.GenerateRandomData(bytes))
            //{
            //    Console.WriteLine(encryptedByte);
            //}

            Console.WriteLine($"Output: { GetApproximateSize(gen.GenerateRandomData(bytes))}");

            Console.ReadKey();
        }

        private static double GetApproximateSize(byte[] input)
        {
            double size;
            object o = new object();
            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, o);
                size = s.Length;
            }

            return size;
        }
    }
}
