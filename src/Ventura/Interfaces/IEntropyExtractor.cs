using System.Collections.Generic;
using System.Threading.Tasks;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;

namespace Ventura.Interfaces
{
    public interface IEntropyExtractor
    {
        Task Run();
        int SourceNumber { get; }
		string SourceName { get; }

		event EntropyAvailabilityHandler EntropyAvailable;
	}

    public delegate void EntropyAvailabilityHandler(Event successfulExtraction);
}
