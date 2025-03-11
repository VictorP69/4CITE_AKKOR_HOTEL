using API.DTO.BookingDto;
using API.Models;
using API.Repository.BookingRepository;
using API.Repository.HotelRepository;
using API.Services.BookingService;
using Moq;
using FluentAssertions;

namespace API.Tests.unit
{
    public class BookingServiceTest
    {
        private readonly Mock<IBookingRepository> mockBookingRepository;
        private readonly Mock<IHotelRepository> mockHotelRepository;
        private readonly BookingService bookingService;

        public BookingServiceTest()
        {
            mockBookingRepository = new Mock<IBookingRepository>();
            mockHotelRepository = new Mock<IHotelRepository>();
            bookingService = new BookingService(mockBookingRepository.Object, mockHotelRepository.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllBookings()
        {
            var bookings = new List<Booking>
            {
                new Booking { Id = Guid.NewGuid(), HotelId = Guid.NewGuid(), UserId = Guid.NewGuid(), CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(3) },
                new Booking { Id = Guid.NewGuid(), HotelId = Guid.NewGuid(), UserId = Guid.NewGuid(), CheckInDate = DateTime.Today.AddDays(5), CheckOutDate = DateTime.Today.AddDays(7) }
            };

            mockBookingRepository.Setup(repo => repo.GetAll()).ReturnsAsync(bookings);

            var result = await bookingService.GetAll();

            result.Should().BeEquivalentTo(bookings);
        }

        [Fact]
        public async Task Get_ShouldReturnBooking_WhenBookingExists()
        {
            var bookingId = Guid.NewGuid();
            var booking = new Booking { Id = bookingId, HotelId = Guid.NewGuid(), UserId = Guid.NewGuid(), CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(3) };

            mockBookingRepository.Setup(repo => repo.Get(bookingId)).ReturnsAsync(booking);

            var result = await bookingService.Get(bookingId);

            result.Should().BeEquivalentTo(booking);
        }

        [Fact]
        public async Task Get_ShouldThrowException_WhenBookingDoesNotExist()
        {
            var bookingId = Guid.NewGuid();
            mockBookingRepository.Setup(repo => repo.Get(bookingId)).ReturnsAsync((Booking)null);

            var exception = await Assert.ThrowsAsync<Exception>(() => bookingService.Get(bookingId));

            Assert.Equal("Cannot find this booking", exception.Message);
        }

        [Fact]
        public async Task GetByUser_ShouldReturnBookings_WhenUserHasBookings()
        {
            var userId = Guid.NewGuid();
            var bookings = new List<Booking>
            {
                new Booking { Id = Guid.NewGuid(), UserId = userId, CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(2) },
                new Booking { Id = Guid.NewGuid(), UserId = userId, CheckInDate = DateTime.Today.AddDays(4), CheckOutDate = DateTime.Today.AddDays(6) }
            };

            mockBookingRepository.Setup(repo => repo.GetByUser(userId)).ReturnsAsync(bookings);

            var result = await bookingService.GetByUser(userId);

            result.Should().BeEquivalentTo(bookings);
        }

        [Fact]
        public async Task Create_ShouldReturnBooking_WhenValidDataProvided()
        {
            var hotelId = Guid.NewGuid();
            var postBookingDto = new PostBookingDto { HotelId = hotelId, UserId = Guid.NewGuid(), CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(3) };
            var createdBooking = new Booking { Id = Guid.NewGuid(), HotelId = hotelId, UserId = postBookingDto.UserId, CheckInDate = postBookingDto.CheckInDate, CheckOutDate = postBookingDto.CheckOutDate };

            mockHotelRepository.Setup(repo => repo.Get(hotelId)).ReturnsAsync(new Hotel { Id = hotelId });
            mockBookingRepository.Setup(repo => repo.Create(postBookingDto, 3)).ReturnsAsync(createdBooking);

            var result = await bookingService.Create(postBookingDto);

            result.Should().BeEquivalentTo(createdBooking);
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenHotelDoesNotExist()
        {
            var postBookingDto = new PostBookingDto { HotelId = Guid.NewGuid(), CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(3) };

            mockHotelRepository.Setup(repo => repo.Get(postBookingDto.HotelId)).ReturnsAsync((Hotel)null);

            var exception = await Assert.ThrowsAsync<Exception>(() => bookingService.Create(postBookingDto));

            Assert.Equal("This hotel doesn't exist", exception.Message);
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenDatesAreInvalid()
        {
            var postBookingDto = new PostBookingDto { HotelId = Guid.NewGuid(), CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today };

            await Assert.ThrowsAsync<Exception>(() => bookingService.Create(postBookingDto));
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenCheckOutDateIsBeforeCheckInDate()
        {
            var hotelId = Guid.NewGuid();
            var postBookingDto = new PostBookingDto
            {
                HotelId = hotelId,
                CheckInDate = DateTime.Today.AddDays(3),
                CheckOutDate = DateTime.Today.AddDays(2)
            };

            mockHotelRepository.Setup(repo => repo.Get(hotelId)).ReturnsAsync(new Hotel { Id = hotelId });

            var exception = await Assert.ThrowsAsync<Exception>(() => bookingService.Create(postBookingDto));
            Assert.Equal("Invalid dates booking reservation", exception.Message);
        }



        [Fact]
        public async Task Update_ShouldReturnUpdatedBooking_WhenValidDataProvided()
        {
            var bookingId = Guid.NewGuid();
            var putBookingDto = new PutBookingDto { CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(4) };
            var booking = new Booking { Id = bookingId, CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(2) };

            mockBookingRepository.Setup(repo => repo.Get(bookingId)).ReturnsAsync(booking);
            mockBookingRepository.Setup(repo => repo.Update(booking, putBookingDto)).ReturnsAsync(booking);

            var result = await bookingService.Update(bookingId, putBookingDto);

            result.Should().BeEquivalentTo(booking);
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenBookingDoesNotExist()
        {
            var bookingId = Guid.NewGuid();
            var putBookingDto = new PutBookingDto { CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(4) };

            mockBookingRepository.Setup(repo => repo.Get(bookingId)).ReturnsAsync((Booking)null);

            var exception = await Assert.ThrowsAsync<Exception>(() => bookingService.Update(bookingId, putBookingDto));

            Assert.Equal("Cannot find this booking", exception.Message);
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenDatesAreInvalid()
        {
            var bookingId = Guid.NewGuid();
            var putBookingDto = new PutBookingDto { CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today };

            var existingBooking = new Booking { Id = bookingId, CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(2) };
            mockBookingRepository.Setup(repo => repo.Get(bookingId)).ReturnsAsync(existingBooking);

            var exception = await Assert.ThrowsAsync<Exception>(() => bookingService.Update(bookingId, putBookingDto));

            Assert.Equal("Invalid dates booking reservation", exception.Message);
        }


        [Fact]
        public async Task Delete_ShouldReturnDeletedBooking_WhenBookingExists()
        {
            var bookingId = Guid.NewGuid();
            var booking = new Booking { Id = bookingId };

            mockBookingRepository.Setup(repo => repo.Get(bookingId)).ReturnsAsync(booking);
            mockBookingRepository.Setup(repo => repo.Delete(booking)).ReturnsAsync(booking);

            var result = await bookingService.Delete(bookingId);

            result.Should().BeEquivalentTo(booking);
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenBookingDoesNotExist()
        {
            var bookingId = Guid.NewGuid();

            mockBookingRepository.Setup(repo => repo.Get(bookingId)).ReturnsAsync((Booking)null);

            var exception = await Assert.ThrowsAsync<Exception>(() => bookingService.Delete(bookingId));

            Assert.Equal("Cannot find this booking", exception.Message);
        }
    }
}
