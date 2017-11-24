using System;
using System.Text;

namespace Ventura.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var gen = new Generator(Constants.CipherOption.Aes);

            string test = "All your base are belong to us";
            var bytes = Encoding.ASCII.GetBytes(test);

            foreach (var encryptedByte in gen.GenerateData(null, bytes))
            {
                System.Console.WriteLine(encryptedByte);
            }

            System.Console.ReadKey();
        }
    }
}
