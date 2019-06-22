using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ventura.Accumulator;

namespace Ventura.Interfaces
{
    public interface IEventEmitter
    {
        Task<Event> Execute(Func<byte[]> extractionLogic);
    }
}
