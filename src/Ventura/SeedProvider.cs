using System;
using System.Collections.Generic;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Accumulator.EntropyExtractors.Remote;
using Ventura.Interfaces;

using Random = System.Random;

namespace Ventura
{
    /// <summary>
    /// Pseudorandomly picks a an entropy source and runs it once. If an
    /// exception occurs (e.g. the source is down) another source is picked.
    /// </summary>
    public class RemoteSeedProvider: ISeedProvider
    {
        private readonly List<IEntropyExtractor> extractors;

        public RemoteSeedProvider(List<IEntropyExtractor> extractors)
        {
            if (extractors == null)
                throw new ArgumentNullException(nameof(extractors));

            if (extractors.Count == 0)
                throw new ArgumentException(nameof(extractors));

            this.extractors = extractors;
        }
        public byte[] GetBytes()
        {
            var random = new Random();

            for (int i = extractors.Count; i > 0; i--)
            {
                var num = random.Next(0, i);

                if (TryGetEntropy(extractors[num], out var bytes))
                    return bytes;

                extractors.RemoveAt(num);
            }

            throw new InvalidOperationException("Could not get entropic data from remote sources");
        }

        public bool TryGetEntropy(IEntropyExtractor extractor, out byte[] entropy)
        {
            entropy = new byte[32];

            try
            {
                entropy = extractor.GetEntropicData().Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public interface ISeedProvider
    {
        byte[] GetBytes();
        bool TryGetEntropy(IEntropyExtractor extractor, out byte[] entropy);
    }
}
