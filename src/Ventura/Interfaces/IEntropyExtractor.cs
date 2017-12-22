using System;
using System.Collections.Generic;
using System.Text;
using Ventura.Accumulator;

namespace Ventura.Interfaces
{
    public interface IEntropyExtractor
    {
        void Start();
        IEnumerable<IEvent> Events { get; }
    }
}
