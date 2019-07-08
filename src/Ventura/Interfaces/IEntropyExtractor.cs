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
		bool IsHealthy { get; }

		/// <summary>
		/// Triggered when entropic data is available
		/// </summary>
		event EntropyAvailabilityHandler OnEntropyAvailable;
    }

    public delegate void EntropyAvailabilityHandler(Event successfulExtraction);
}
