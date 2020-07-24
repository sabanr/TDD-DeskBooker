using DeskBooker.Core.Domain;
using FluentAssertions;
using System;
using Xunit;

namespace DeskBooker.Core.Processor {
	public class DeskBookingRequestProcessorTests {

		private readonly DeskBookingRequestProcessor _processor;

		public DeskBookingRequestProcessorTests() {
			_processor = new DeskBookingRequestProcessor();
		}

		[Fact]
		public void Should_return_deskbooking_result_with_request_values() {

			// arrange
			var request = new DeskBookingRequest {
				FirstName = "Roberto",
				LastName = "Saban",
				Email = "roberto.saban@me.to",
				Date = new DateTime(2020, 01, 28)
			};

			// act
			DeskBookingResult result = _processor.BookDesk(request);

			// assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(request);
			//Assert.Equal(request.FirstName, result.FirstName);
			//Assert.Equal(request.LastName, result.LastName);
			//Assert.Equal(request.Email, result.Email);
			//Assert.Equal(request.Date, result.Date);

		}

		[Fact]
		public void Should_throw_exception_if_request_is_null() {
			_processor.Invoking(p => p.BookDesk(null))
				.Should()
				.Throw<ArgumentNullException>()
				.And.ParamName.Should().Be("request");

		}

	}
}
