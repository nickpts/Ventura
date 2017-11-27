namespace Ventura.Console
{
    using System;
    using System.Text;

    using Console = System.Console;

    internal class Program
    {
        static void Main(string[] args)
        {
            var gen = new Generator(Constants.CipherOption.TwoFish);

            string test = "All your base are belong to us";
            var bytes = Encoding.ASCII.GetBytes(test);

            foreach (var encryptedByte in gen.GenerateData(bytes))
            {
                Console.WriteLine(encryptedByte);
            }

            Console.ReadKey();
        }
    }
}
