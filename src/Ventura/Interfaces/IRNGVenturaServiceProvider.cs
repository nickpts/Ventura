using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    public interface IRNGVenturaServiceProvider : IDisposable
    {
	    void Initialise();
	    void GetBytes(byte[] input);

	    int GetRandomNumber(int min, int max);

	    int[] GetRandomNumbers(int min, int max, int length);

	    double GetRandomNumber(int roundToDecimals);

	    double[] GetRandomNumbers(int roundToDecimals, int length);
    }
}
