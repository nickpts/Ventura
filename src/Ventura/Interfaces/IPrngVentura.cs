using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    public interface IPrngVentura
    {
	    byte[] GetRandomData(byte[] input);
    }
}
