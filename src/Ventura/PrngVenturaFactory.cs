using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura
{
    public class PrngVenturaFactory
    {
        public static IPrngVentura Create() => Create(Cipher.Aes, ReseedEntropySourceGroup.Full, null, default(CancellationToken));

        public static IPrngVentura Create(Cipher cipher) => Create(cipher, ReseedEntropySourceGroup.Full, null, default(CancellationToken));

        public static IPrngVentura Create(Cipher cipher, ReseedEntropySourceGroup sourceGroup) => Create(cipher, sourceGroup, null, default(CancellationToken));

        public static IPrngVentura Create(Cipher cipher, ReseedEntropySourceGroup sourceGroup, byte[] seed, CancellationToken entropyCancellationToken)
		{ 
	        var extractors = new List<IEntropyExtractor>();

            switch (sourceGroup)
            {
                case ReseedEntropySourceGroup.Local:
                    extractors.Add(new GarbageCollectorExtractor(0));
                    extractors.Add(new AppDomainExtractor(1));
                    break;
                case ReseedEntropySourceGroup.Remote:
                    extractors.Add(new RemoteQuantumRngExtractor(0));
                    break;
                case ReseedEntropySourceGroup.Full:
                    extractors.Add(new GarbageCollectorExtractor(0));
                    extractors.Add(new AppDomainExtractor(1));
                    extractors.Add(new RemoteQuantumRngExtractor(2));
                    break;
                default:
	                throw new ArgumentOutOfRangeException(nameof(sourceGroup), sourceGroup, null);
            }

            return new PrngVentura(new VenturaAccumulator(extractors, entropyCancellationToken), new VenturaGenerator(cipher, seed));
		}
    }
}