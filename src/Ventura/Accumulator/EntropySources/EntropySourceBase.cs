using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities.Collections;
using Ventura.Interfaces;

namespace Ventura.Accumulator.EntropySources
{
    public abstract class EntropySourceBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNumber"></param>
        protected EntropySourceBase(int sourceNumber)
        {
                
        }

        public virtual string SourceName { get; set; } = string.Empty;

    }
}
