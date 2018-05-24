using System.Collections.Generic;
using Ventura.Accumulator;

namespace Ventura.Interfaces
{
    public interface IEntropyExtractor
    {
        void Start();
        IEnumerable<Event> Events { get; }
        int SourceNumber { get; }
    }
}
