using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura.Interfaces
{
    public interface IPRNGVenturaService
    {
        void InitialisePRNG();
        byte[] GetRandomData();
        int GetRandomNumber();
        int[] GetRandomNumbers();
        string GetRandomString(int length);
        string[] GetRandomStrings(int length);
        string[] GetRandomStrings(int minLength, int maxLength);
    }
}
