using System;
using System.Collections.Generic;
using System.Text;

using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura
{
    public class PRNGVenturaServiceFactory
    {
        public static IPRNGVenturaService CreatePrng() => CreatePrng(Cipher.Aes, ReseedEntropySources.Full, null);

        public static IPRNGVenturaService CreatePrng(Cipher cipher) =>
            CreatePrng(cipher, ReseedEntropySources.Full, null);

        public static IPRNGVenturaService CreatePrng(Cipher cipher, ReseedEntropySources sources) =>
            CreatePrng(cipher, sources, null);
        
        public static IPRNGVenturaService CreatePrng(Cipher cipher, ReseedEntropySources sources, byte[] seed)
        {
            var extractors = new List<IEntropyExtractor>();

            switch (sources)
            {
                case ReseedEntropySources.Local:
                    extractors.AddRange(GetLocalEntropyExtractors());
                    break;
                case ReseedEntropySources.Remote:
                    extractors.AddRange(GetRemoteEntropyExtractors());
                    break;
                case ReseedEntropySources.Full:
                    extractors.AddRange(GetLocalEntropyExtractors());
                    extractors.AddRange(GetRemoteEntropyExtractors());
                    break;
            }

            IAccumulator accumulator = new VenturaAccumulator(extractors);
            IGenerator generator = new VenturaGenerator(cipher, seed);

            var prng = new PRNGVenturaService(accumulator, generator);
            prng.InitialisePRNG();

            return prng;
        }

        public static List<IEntropyExtractor> GetLocalEntropyExtractors() => 
            new List<IEntropyExtractor>() {new GarbageCollectorExtractor(0)};
        
        public static List<IEntropyExtractor> GetRemoteEntropyExtractors() =>
            new List<IEntropyExtractor>() {new RemoteQuantumRngExtractor(1)};
    }
}
