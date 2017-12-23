using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ventura.Interfaces
{
    public interface IEventEmitter
    {
        void Execute(Task<byte[]> extractionLogic);
    }
}
