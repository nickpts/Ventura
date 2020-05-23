using System;
using System.IO;

namespace Ventura.Samples
{
    class Program
    {
        static void Main(string[] args)
        {

            var output = new byte[16];

            // memory stream, RNG will be seeded automatically and seeds discarded at the end of the operation
            // Cipher by default is AES with full entropy reseed sources
            using var prng = RNGVenturaProviderFactory.Create(new MemoryStream());
            prng.GetBytes(output);

            // uses a file stream to read the seed from file. will write seed to same file at the end of scope
            using var prng2 = RNGVenturaProviderFactory.Create(new FileStream("seed.bin",FileMode.OpenOrCreate));
            prng2.GetBytes(output);

            // support for additional ciphers and reseed entropy sources can be either local, remote or both
            using var prng3 = 
                RNGVenturaProviderFactory.Create(new FileStream("seed.bin", FileMode.OpenOrCreate), Cipher.BlowFish, ReseedEntropySourceGroup.Local);
            prng3.GetBytes(output);

            // seeds the RNG with a combination seed from remote entropy sources - slower than the above options.
            using var prng4 = RNGVenturaProviderFactory.CreateSeeded(); 
            prng4.GetBytes(output);

            // additional initialisation options
            using var prng5 = RNGVenturaProviderFactory.CreateSeeded(Cipher.Serpent, ReseedEntropySourceGroup.Local);
            prng5.GetBytes(output);

            // special initialisation, creates an instance of RandomNumberGenerator so Ventura can be used as drop-in replacement 
            // in solutions that already use RandomNumberGenerator in a source compatible change
            using var prng6 =
                RNGVenturaProviderFactory.CreateRng(new MemoryStream(), Cipher.Aes, ReseedEntropySourceGroup.Full);
            prng6.GetBytes(output);

            using var prng7 = RNGVenturaProviderFactory.CreateSeeded();
            var num = prng7.Next(1, 10); // get a 32-bit integer between 1 and 10 both values inclusive

            var numArray = prng7.NextArray(1, 10, 100); // get an array of 100 32-bit integers between 1 and 10 both values inclusive
        }
    }
}
