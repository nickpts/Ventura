using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    public interface IPRNGVenturaService
    {
	    byte[] GetRandomData(byte[] input);
        int GetRandomNumber();
        int[] GetRandomNumbers();
        string GetRandomString(int length);
        string[] GetRandomStrings(int length);
        string[] GetRandomStrings(int minStringLength, int maxStringLength, int arrayLength);
    }
}
