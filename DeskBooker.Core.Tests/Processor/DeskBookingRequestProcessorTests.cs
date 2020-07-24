using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeskBooker.Core.Processor {
	public class DeskBookingRequestProcessorTests {
		
		#region Fields

		private readonly DeskBookingRequestProcessor _processor;
		private readonly DeskBookingRequest _request;
		private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
		private readonly Mock<IDeskRepository> _repositoryMock;
		private readonly List<Desk> _availableDesks = new List<Desk> {
			new Desk { Id = 7 }
		};

		#endregion

		#region Constructors

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

		#endregion

		#region Tests

		[Fact]
		public void ShouldReturnDeskBookingResulWithRequestValues() {
#pragma warning restore IDE1006 // Naming Styles
			DeskBookingResult result = _processor.BookDesk(_request);

			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(_request);
			//Assert.Equal(request.FirstName, result.FirstName);
			//Assert.Equal(request.LastName, result.LastName);
			//Assert.Equal(request.Email, result.Email);
			//Assert.Equal(request.Date, result.Date);

		}

		[Fact]
		public void ShouldThrowExceptionIfRequestIsNull() {
			_processor.Invoking(p => p.BookDesk(null))
				.Should()
				.Throw<ArgumentNullException>()
				.And.ParamName.Should().Be("request");
		}

		[Fact]
		public void ShouldSaveDeskBooking() {
			DeskBooking savedDeskBooking = null;

			_deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
				.Callback<DeskBooking>(deskBooking => {
					savedDeskBooking = deskBooking;
				});

			_processor.BookDesk(_request);

			_deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
			savedDeskBooking.Should().NotBeNull();
			savedDeskBooking.Should().BeEquivalentTo(_request);
			savedDeskBooking.DeskId.Should().Be(_availableDesks.First().Id);
		}

		[Fact]
		public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable() {
			_availableDesks.Clear();

			_processor.BookDesk(_request);

			_deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
		}

		[Theory]
		[InlineData(DeskBookingResultCode.Success, true)]
		[InlineData(DeskBookingResultCode.NoDeskAvailable, false)]
		public void ShouldReturnExpectedResultCode(DeskBookingResultCode expectedResultCode, bool isDeskAvailable) {
			if (!isDeskAvailable) {
				_availableDesks.Clear();
			}

			DeskBookingResult result = _processor.BookDesk(_request);

			result.Code.Should().Be(expectedResultCode);
		}

		[Theory]
		[InlineData(5, true)]
		[InlineData(null, false)]
		public void ShouldReturnExpectedDeskBookingId(int? expectedDeskBookingId, bool isDeskAvailable) {
			if (!isDeskAvailable) {
				_availableDesks.Clear();
			} else {
				_deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
					.Callback<DeskBooking>(deskBooking => {
						deskBooking.Id = expectedDeskBookingId.Value;
					});
			}

			DeskBookingResult result = _processor.BookDesk(_request);

			result.DeskBookingId.Should().Be(expectedDeskBookingId);
		}
		#endregion
	}
}
