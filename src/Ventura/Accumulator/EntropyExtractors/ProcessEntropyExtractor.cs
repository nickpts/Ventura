using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ventura.Interfaces;


namespace Ventura.Accumulator.EntropyExtractors
{
	/// <summary>
	/// Collects statistics from the current process and turns them into a byte array
	/// </summary>
	public class ProcessEntropyExtractor : EntropyExtractorBase, IEntropyExtractor
	{
		private readonly Process process = Process.GetCurrentProcess();

		public ProcessEntropyExtractor(int sourceNumber) : base(sourceNumber)
		{

		}


		public override string SourceName { get; protected set; } = ".NET current process entropy extractor";

		protected override Func<byte[]> ExtractEntropicData()
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
