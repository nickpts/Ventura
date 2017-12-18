using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Accumulator
{
    public class Pool
    {
        private int poolNumber;
        private byte[] hashBuffer;

        public Pool(int poolNumber) => this.poolNumber = poolNumber;
        
        public byte[] Hash { get; set; }

        public void ProcessEvent()
        {
            
        }
    }
}
