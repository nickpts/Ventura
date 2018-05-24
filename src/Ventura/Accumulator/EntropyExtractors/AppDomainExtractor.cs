using System;
using System.Threading.Tasks;

using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
    public class AppDomainExtractor: EntropyExtractorBase, IEntropyExtractor
    {
        public AppDomainExtractor(int sourceNumber) : base(sourceNumber)
        {
        }

        public override string SourceName { get; protected set; } = ".NET Application domain entropy extractor";

        protected override Task<byte[]> ExtractEntropicData()
        {
            byte[] extraction()
            {
                AppDomain.MonitoringIsEnabled = true;

                var domain = AppDomain.CurrentDomain;
                
                var survivedMemory = domain.MonitoringSurvivedMemorySize;
                var totalMemory = domain.MonitoringTotalAllocatedMemorySize;
                var totalProcessorTime = domain.MonitoringTotalProcessorTime;

                return BitConverter.GetBytes(survivedMemory + totalMemory + totalProcessorTime.Ticks);
            }

            return Task.Run((Func<byte[]>)extraction);
        }
    }
}
