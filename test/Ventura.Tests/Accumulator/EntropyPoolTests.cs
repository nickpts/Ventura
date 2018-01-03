using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using FluentAssertions;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Interfaces;

namespace Ventura.Tests.Accumulator
{
    [TestClass]
    public class EntropyPoolTests
    {
        private EntropyPool testPool = new EntropyPool(0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EntropyPool_ThrowsArgumentException_If_Source_Outside_Boundary()
        {
            testPool.AddEventData(-1, new byte[10]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EntropyPool_ThrowsArgumentException_If_EventData_Bigger_Than_Allowed()
        {
            testPool.AddEventData(0, new byte[33]);
        }
    }
}
