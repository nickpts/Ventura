using System;

using NUnit.Framework;
using Ventura.Accumulator;

namespace Ventura.Tests.Accumulator
{
    [TestFixture()]
    public class EntropyPoolTests
    {
        private EntropyPool testPool = new EntropyPool(0);

        [Test]
        public void EntropyPool_ThrowsArgumentException_If_Source_Outside_Boundary()
        {
            Assert.Throws(typeof(ArgumentException), () => testPool.AddEventData(-1, new byte[10]));
        }

        [Test]
        public void EntropyPool_ThrowsArgumentException_If_EventData_Bigger_Than_Allowed()
        {
            Assert.Throws(typeof(ArgumentException), () => testPool.AddEventData(0, new byte[33]));
        }
    }
}
