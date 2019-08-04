using System;
using System.Diagnostics;
using System.Linq;
using Ventura.Interfaces;


namespace Ventura.Accumulator.EntropyExtractors.Local
{
	/// <summary>
	/// Collects statistics from the current process and turns them into a byte array
	/// </summary>
	internal class ProcessEntropyExtractor : EntropyExtractorBase, IEntropyExtractor
	{
		private readonly Process process = Process.GetCurrentProcess();

		public ProcessEntropyExtractor(IEventEmitter eventEmitter) : base(eventEmitter)
		{
		}

		public override string SourceName { get; protected set; } = nameof(ProcessEntropyExtractor);

        public override Func<byte[]> GetEntropicData()
		{
			return () =>
			{
				process.Refresh();

				var pTime = BitConverter.GetBytes(process.TotalProcessorTime.Ticks).Take(6);
				var uTime = BitConverter.GetBytes(process.UserProcessorTime.Ticks).Take(6);
				var memory = BitConverter.GetBytes(process.VirtualMemorySize64).Take(6);
				var pMemory = BitConverter.GetBytes(process.PagedMemorySize64).Take(6);
				var wMemory = BitConverter.GetBytes(process.WorkingSet64).Take(6);

				var total = pTime.Concat(uTime).Concat(memory).Concat(pMemory).Concat(wMemory).ToArray();

				return total;
			};
		}

		public void Dispose() => process.Dispose();
	}
}
