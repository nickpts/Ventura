using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropyExtractors
{
    /// <summary>
    /// 
    /// </summary>
    public class AnuQuantumRngExtractor : EntropyExtractorBase, IEntropyExtractor
    {
        public AnuQuantumRngExtractor(int sourceNumber): base(sourceNumber)
        {   
        }

        protected override Task<byte[]> ExtractEntropicData()
        {

            return Task.Run(() => new byte[32]);

        }
    }
}
