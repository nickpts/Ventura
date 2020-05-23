using System;
using System.IO;
using CommandLine;

namespace Ventura.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<RandomNumberOptions, RandomNumberArrayOptions>(args)
                .MapResult(
                    (RandomNumberOptions opts) =>
                    {
                        var stream = new FileStream(opts.SeedPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        using var prng = RNGVenturaProviderFactory.Create(stream, opts.Cipher, opts.EntropyGroup);

                        var rn = prng.Next(opts.Min, opts.Max);
                        Console.WriteLine(rn);
                        
                        return 0;
                    },

                    (RandomNumberArrayOptions opts) =>
                    {
                        var stream = new FileStream(opts.SeedPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        var array = new int[opts.ArrayLength];

                        using var prng = RNGVenturaProviderFactory.Create(stream, opts.Cipher, opts.EntropyGroup);

                        array = prng.NextArray(opts.Min, opts.Max, opts.ArrayLength);

                        for (int i = 0; i < array.Length; i++)
                        {
                            Console.Write(array[i] + " ");
                        }

                        return 0;
                    },

                    errs => 1
                );
        }
    }
}