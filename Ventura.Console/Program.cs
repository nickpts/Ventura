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

            System.Console.WriteLine(gen.GenerateData(null, bytes));
            System.Console.ReadKey();
        }
    }
}
