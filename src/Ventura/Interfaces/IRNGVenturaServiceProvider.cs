using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    public interface IRNGVenturaServiceProvider : IDisposable
    {
	    void Initialise();
	    void GetBytes(byte[] input);

	    int Next(int min, int max);

	    int[] NextArray(int min, int max, int length);

	    double NextDouble(int roundToDecimals);

	    double[] NextDoubleArray(int roundToDecimals, int length);
    }
}
