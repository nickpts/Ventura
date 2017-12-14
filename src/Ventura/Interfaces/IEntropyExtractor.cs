using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    public interface IEntropyExtractor
    {
        void Start();
        IEnumerable<IEvent> Events { get; }
    }
}
