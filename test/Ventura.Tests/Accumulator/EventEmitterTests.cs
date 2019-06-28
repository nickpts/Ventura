using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Ventura.Accumulator;

namespace Ventura.Tests.Accumulator
{
	[TestFixture]
	public class EventEmitterTests
	{
		private EventEmitter emitter;

		[SetUp]
		public void Setup()
		{
			emitter = new EventEmitter(0);
		}

		[Test]
		public void EventEmitter_Returns_ByteArray_From_EntropyFunction()
		{
			var data = emitter.Execute(() => new byte[2] { 0, 1 } );

			data.Result.ExtractionSuccessful.Should().BeTrue();
			data.Result.SourceNumber.Should().Be(0);
			data.Result.Data.Length.Should().Be(32);
		}

		[Test]
		public void EventEmitter_Encapsulates_Exception()
		{
			var data = emitter.Execute(() => throw new InvalidOperationException());

			data.Result.ExtractionSuccessful.Should().BeFalse();
			data.Result.Exception.Should().BeOfType<InvalidOperationException>();
		}

		[Test]
		public void EventEmitter_Returns_Data_In_Appropriate_Format()
		{
			// TODO
		}
	}
}
