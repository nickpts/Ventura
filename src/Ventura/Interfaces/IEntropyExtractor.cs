using System;
using System.Threading.Tasks;
using Ventura.Accumulator;

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

        Func<byte[]> GetEntropicData();
    }

    public delegate void EntropyAvailabilityHandler(Event successfulExtraction);
}
