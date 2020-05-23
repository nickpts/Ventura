using System;
using System.Threading.Tasks;
using Ventura.Accumulator;

namespace Ventura.Interfaces
{
    public interface IEntropyExtractor
    {
        /// <summary>
        /// Start extractor 
        /// </summary>
        Task Run();

        /// <summary>
        /// Sources are numbered according to spec
        /// </summary>
        int SourceNumber { get; }

        /// <summary>
        /// Source name
        /// </summary>
		string SourceName { get; }

        /// <summary>
        /// Indicates if extractor has been successfully producing entropic data
        /// </summary>
		bool IsHealthy { get; }

        /// <summary>
		/// Triggered when entropic data is available
		/// </summary>
		event EntropyAvailabilityHandler OnEntropyAvailable;

        /// <summary>
        /// Main functionality for extracting entropic data
        /// </summary>
        Func<byte[]> GetEntropicData();
    }

    public delegate void EntropyAvailabilityHandler(Event successfulExtraction);
}
