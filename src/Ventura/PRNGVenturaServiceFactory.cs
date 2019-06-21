using System;
using System.Collections.Generic;
using System.Text;

using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura
{
    public class PrngVenturaServiceFactory
    {
        public static IPRNGVenturaService CreatePrng() => CreatePrng(Cipher.Aes, ReseedEntropySources.Full, null);

        public static IPRNGVenturaService CreatePrng(Cipher cipher) => CreatePrng(cipher, ReseedEntropySources.Full, null);

        public static IPRNGVenturaService CreatePrng(Cipher cipher, ReseedEntropySources sources) => CreatePrng(cipher, sources, null);

        public static IPRNGVenturaService CreatePrng(Cipher cipher, ReseedEntropySources sources, byte[] seed)
		{ 
	        var extractors = new List<IEntropyExtractor>();

            switch (sources)
            {
                case ReseedEntropySources.Local:
                    extractors.Add(new GarbageCollectorExtractor(0));
                    extractors.Add(new AppDomainExtractor(1));
                    break;
                case ReseedEntropySources.Remote:
                    extractors.Add(new RemoteQuantumRngExtractor(0));
                    break;
                case ReseedEntropySources.Full:
                    extractors.Add(new GarbageCollectorExtractor(0));
                    extractors.Add(new AppDomainExtractor(1));
                    extractors.Add(new RemoteQuantumRngExtractor(2));
                    break;
                default:
	                throw new ArgumentOutOfRangeException(nameof(sources), sources, null);
            }

            IAccumulator accumulator = new VenturaAccumulator(extractors);
            IGenerator generator = new VenturaGenerator(cipher, seed);

            var prng = new PrngVenturaService(accumulator, generator);
            prng.InitialisePRNG();

            return prng;
        }
    }
}