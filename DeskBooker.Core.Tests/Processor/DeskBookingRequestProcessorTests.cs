using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeskBooker.Core.Processor {
	public class DeskBookingRequestProcessorTests {

		private readonly DeskBookingRequestProcessor _processor;
		private readonly DeskBookingRequest _request;
		private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
		private readonly Mock<IDeskRepository> _repositoryMock;
		private readonly List<Desk> _availableDesks = new List<Desk> {
			new Desk()
		};

		public DeskBookingRequestProcessorTests() {
			_deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
			_repositoryMock = new Mock<IDeskRepository>();

			_request = new DeskBookingRequest {
				FirstName = "Roberto",
				LastName = "Saban",
				Email = "roberto.saban@me.to",
				Date = new DateTime(2020, 01, 28)
			};

			_repositoryMock.Setup(x => x.GetAvailableDesks(_request.Date))
				.Returns(_availableDesks);

			_processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object, _repositoryMock.Object);
		}

		[Fact]
		public void Should_return_deskbooking_result_with_request_values() {
			DeskBookingResult result = _processor.BookDesk(_request);

			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(_request);
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

		[Fact]
		public void Should_save_deskbooking() {
			DeskBooking savedDeskBooking = null;

			_deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
				.Callback<DeskBooking>(deskBooking => {
					savedDeskBooking = deskBooking;
				});

			_processor.BookDesk(_request);

			_deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
			savedDeskBooking.Should().NotBeNull();
			savedDeskBooking.Should().BeEquivalentTo(_request);
		}

		[Fact]
		public void Should_not_save_deskbooking_if_no_desk_is_available() {
			_availableDesks.Clear();

			_processor.BookDesk(_request);

			_deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
		}
	}
}
