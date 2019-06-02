﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Ventura.Interfaces;

namespace Ventura
{
    internal class PRNGVenturaService : IPRNGVenturaService
    {
        private readonly IAccumulator accumulator;
        private readonly IGenerator generator;
        private DateTimeOffset lastReseedTime = DateTimeOffset.MinValue;
        private int reseedCounter = 0;

        public PRNGVenturaService(IAccumulator accumulator, IGenerator generator)
        {
            this.accumulator = accumulator;
            this.generator = generator;
        }

        public void InitialisePRNG()
        {
            while (!accumulator.HasEnoughEntropy)
            {
                accumulator.Distribute();

                Debug.WriteLine("Not enough entropy... waiting");
                Task.Delay(500);
            }

            Debug.WriteLine("I have enough entropy!");
        }

        public void Reseed(byte[] seed)
        {
            generator.Reseed(seed);
            lastReseedTime = DateTimeOffset.UtcNow;
            reseedCounter++;
        }

        public byte[] GetRandomData(byte[] input)
        {
			return generator.GenerateData(input);
        }

        public int GetRandomNumber()
        {
            throw new NotImplementedException();
        }

        public int[] GetRandomNumbers()
        {
            throw new NotImplementedException();
        }

        public string GetRandomString(int length)
        {
            throw new NotImplementedException();
        }

        public string[] GetRandomStrings(int length)
        {
            throw new NotImplementedException();
        }

        public string[] GetRandomStrings(int minStringLength, int maxStringLength, int arrayLength)
        {
            throw new NotImplementedException();
        }
    }
}