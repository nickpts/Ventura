﻿using System;
using System.Runtime.CompilerServices;

using static Ventura.Constants;

[assembly: InternalsVisibleTo("Ventura.Tests")]
[assembly: InternalsVisibleTo("Ventura.Tests.Visualisation")]
namespace Ventura.Generator
{
    internal class GeneratorState
    {
        public int Counter { get; internal set; }
        public byte[] Key { get; internal set; }
		public string StringKey => System.Text.Encoding.Default.GetString(Key);
        public bool Seeded { get; internal set; }

        public byte[] TransformCounterToByteArray()
        {
            var destArray = new byte[CipherBlockSize];
            var sourceArray = BitConverter.GetBytes(Counter);

            Array.Copy(sourceArray, destArray, sourceArray.Length);

            return destArray;
        }

		public string TransformByteArrayCounterToString()
		{
			var counterArray = TransformCounterToByteArray();

			return System.Text.Encoding.Default.GetString(counterArray);
		}
    }
}
