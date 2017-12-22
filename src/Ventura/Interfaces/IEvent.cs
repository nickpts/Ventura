using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    internal interface IEvent
    {
        byte[] Data { get; set; }
        bool ExtractionSuccessful { get; set; }
        AggregateException Exception { get; set; }
    }
}
