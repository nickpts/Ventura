using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ventura.Interfaces
{
    public delegate void EntropyAvailabilityHander(IEvent successfulExtraction);
    public delegate void EventFailureHandler(IEvent failedExtraction);

    public interface IEventEmitter
    {
        void Execute(Task<byte[]> extractionLogic);
        event EntropyAvailabilityHander OnEntropyAvailable;
        event EventFailureHandler OnFailedEvent;
    }
}
