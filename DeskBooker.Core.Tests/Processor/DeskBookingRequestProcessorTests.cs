using System;
using Xunit;

namespace DeskBooker.Core.Processor {
	public class DeskBookingRequestProcessorTests {

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
			var processor = new DeskBookingRequestProcessor();
			DeskBookingResult result = processor.BookDesk(request);

			// asset
			Assert.NotNull(result);
			Assert.Equal(request.FirstName, result.FirstName);
			Assert.Equal(request.LastName, result.LastName);
			Assert.Equal(request.Email, result.Email);
			Assert.Equal(request.Date, result.Date);

		}

	}
}
