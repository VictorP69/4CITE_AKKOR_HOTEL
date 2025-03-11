using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.Services.BookingService;
using API.Services.UserService;
using API.Models;
using API.DTO.BookingDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using System.Text.Json;

namespace API.Tests.Functional
{
    public class BookingControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly ITestOutputHelper _output;

        public BookingControllerTest(ITestOutputHelper output)
        {
            _mockUserService = new Mock<IUserService>();
            _mockBookingService = new Mock<IBookingService>();
            _output = output;
        }

        private BookingController SetUpControllerWithClaims(UserRole role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "12345"),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            var controller = new BookingController(_mockBookingService.Object, _mockUserService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            return controller;
        }

        [Fact]
        public async Task Index_ShouldReturnBookings_WhenUserIsAdmin()
        {
            var controller = SetUpControllerWithClaims(UserRole.Admin);

            var mockCurrentUser = new User
            {
                Id = "12345",
                Role = UserRole.Admin,
                Pseudo = "John"
            };

            _mockUserService.Setup(service => service.GetCurrentUser()).ReturnsAsync(mockCurrentUser);

            var mockBookings = new List<Booking>
            {
                new Booking { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), HotelId = Guid.NewGuid(), CheckInDate = DateTime.Now.AddDays(1), CheckOutDate = DateTime.Now.AddDays(5) },
                new Booking { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), HotelId = Guid.NewGuid(), CheckInDate = DateTime.Now.AddDays(1), CheckOutDate = DateTime.Now.AddDays(5) }
            };

            _mockBookingService.Setup(service => service.GetAll()).ReturnsAsync(mockBookings);

            var result = await controller.Index();

            result.Should().BeOfType<ApiResponse<List<Booking>>>();
            var apiResponse = result as ApiResponse<List<Booking>>;
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().BeEquivalentTo(mockBookings);
            apiResponse.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Index_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
        {
            var controller = SetUpControllerWithClaims(UserRole.User);

            var mockCurrentUser = new User
            {
                Id = "12345",
                Role = UserRole.User,
                Pseudo = "John"
            };

            _mockUserService.Setup(service => service.GetCurrentUser()).ReturnsAsync(mockCurrentUser);
            var result = await controller.Index();

            result.Should().BeOfType<ApiResponse<List<Booking>>>();
            var apiResponse = result as ApiResponse<List<Booking>>;
            apiResponse.Success.Should().BeFalse();
            apiResponse.Message.Should().Be("You cannot read all bookings");
            apiResponse.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedBooking_WhenUserIsAuthorized()
        {
            var controller = SetUpControllerWithClaims(UserRole.User);

            var postBookingDto = new PostBookingDto
            {
                HotelId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(5)
            };

            var createdBooking = new Booking
            {
                Id = Guid.NewGuid(),
                HotelId = postBookingDto.HotelId,
                UserId = postBookingDto.UserId,
                CheckInDate = postBookingDto.CheckInDate,
                CheckOutDate = postBookingDto.CheckOutDate
            };

            _mockBookingService.Setup(service => service.Create(postBookingDto)).ReturnsAsync(createdBooking);

            var result = await controller.Create(postBookingDto);
            _output.WriteLine("result = " + JsonSerializer.Serialize(result));

            result.Should().BeOfType<ApiResponse<Booking>>();
            var apiResponse = result as ApiResponse<Booking>;
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().BeEquivalentTo(createdBooking);
            apiResponse.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Delete_ShouldReturnUnauthorized_WhenUserTryToDeleteAnotherUsersBooking()
        {
            var controller = SetUpControllerWithClaims(UserRole.User);

            var bookingId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var booking = new Booking
            {
                Id = bookingId,
                UserId = anotherUserId,
                HotelId = Guid.NewGuid(),
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(5)
            };

            _mockUserService.Setup(service => service.GetCurrentUser()).ReturnsAsync(new User
            {
                Id = currentUserId.ToString(),
                Role = UserRole.User
            });

            _mockBookingService.Setup(service => service.Get(bookingId)).ReturnsAsync(booking);

            var result = await controller.Delete(bookingId);

            result.Should().BeOfType<ApiResponse<Booking>>();
            var apiResponse = result as ApiResponse<Booking>;
            apiResponse.Success.Should().BeFalse();
            apiResponse.Message.Should().Be("You can only delete your booking");
            apiResponse.StatusCode.Should().Be(401);
        }
    }
}
