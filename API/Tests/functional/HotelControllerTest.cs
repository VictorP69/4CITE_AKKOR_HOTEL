using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.Services.HotelService;
using API.Services.UserService;
using API.Models;
using API.DTO.HotelDto;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;
using System.Text.Json;

namespace API.Tests.Functional
{
    public class HotelControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IHotelService> _mockHotelService;
        private readonly ITestOutputHelper _output;

        public HotelControllerTest(ITestOutputHelper output)
        {
            _mockUserService = new Mock<IUserService>();
            _mockHotelService = new Mock<IHotelService>();
            _output = output;
        }

        private HotelController SetUpControllerWithClaims(UserRole role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "12345"),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            var controller = new HotelController(_mockUserService.Object, _mockHotelService.Object);

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
        public async Task Index_ShouldReturnHotels_WhenUserIsAuthorized()
        {
            var controller = SetUpControllerWithClaims(UserRole.Admin);

            var mockHotels = new List<Hotel>
            {
                new Hotel { Id = Guid.NewGuid(), Name = "Hotel A" },
                new Hotel { Id = Guid.NewGuid(), Name = "Hotel B" }
            };

            _mockHotelService.Setup(service => service.GetAll()).ReturnsAsync(mockHotels);

            var result = await controller.Index();

            result.Should().BeOfType<ApiResponse<List<Hotel>>>();
            var apiResponse = result as ApiResponse<List<Hotel>>;
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().BeEquivalentTo(mockHotels);
            apiResponse.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Details_ShouldReturnHotel_WhenUserIsAuthorized()
        {
            var controller = SetUpControllerWithClaims(UserRole.Admin);
            var hotelId = Guid.NewGuid();
            var mockHotel = new Hotel { Id = hotelId, Name = "Hotel A" };

            _mockHotelService.Setup(service => service.Get(hotelId)).ReturnsAsync(mockHotel);

            var result = await controller.Details(hotelId);

            result.Should().BeOfType<ApiResponse<Hotel>>();
            var apiResponse = result as ApiResponse<Hotel>;
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().BeEquivalentTo(mockHotel);
            apiResponse.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedHotel_WhenUserIsAdmin()
        {
            var controller = SetUpControllerWithClaims(UserRole.Admin);

            var mockCurrentUser = new User
            {
                Id = "12345",
                Role = UserRole.Admin,
                Pseudo = "John"
            };

            var postHotelDto = new PostHotelDto
            {
                Name = "New Hotel",
                Location = "Paris",
                Description = "A beautiful hotel in Paris.",
                NightPrice = 150.0,
                Pictures = new List<IFormFile> { }
            };
            _mockUserService.Setup(service => service.GetCurrentUser()).ReturnsAsync(mockCurrentUser);

            var createdHotel = new Hotel { Id = Guid.NewGuid(), Name = postHotelDto.Name, Location = postHotelDto.Location, Description = postHotelDto.Description, NightPrice = postHotelDto.NightPrice, PictureList = new List<HotelBlob> { } };

            _mockHotelService.Setup(service => service.Create(It.IsAny<PostHotelDto>())).ReturnsAsync(createdHotel);

            var result = await controller.Create(postHotelDto);
            result.Should().BeOfType<ApiResponse<Hotel>>();
            var apiResponse = result as ApiResponse<Hotel>;
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().BeEquivalentTo(createdHotel);
            apiResponse.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Edit_ShouldReturnUpdatedHotel_WhenUserIsAdmin()
        {
            var controller = SetUpControllerWithClaims(UserRole.Admin);

            var mockCurrentUser = new User
            {
                Id = "12345",
                Role = UserRole.Admin,
                Pseudo = "John"
            };

            var hotelId = Guid.NewGuid();
            var patchHotelDto = new PatchHotelDto { Name = "Updated Hotel" };

            _mockUserService.Setup(service => service.GetCurrentUser()).ReturnsAsync(mockCurrentUser);

            var updatedHotel = new Hotel { Id = hotelId, Name = "Updated Hotel", Location = "Paris" };

            _mockHotelService.Setup(service => service.Update(hotelId, patchHotelDto)).ReturnsAsync(updatedHotel);

            var result = await controller.Edit(hotelId, patchHotelDto);

            result.Should().BeOfType<ApiResponse<Hotel>>();
            var apiResponse = result as ApiResponse<Hotel>;
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().BeEquivalentTo(updatedHotel);
            apiResponse.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Delete_ShouldReturnDeletedHotel_WhenUserIsAdmin()
        {
            var controller = SetUpControllerWithClaims(UserRole.Admin);

            var mockCurrentUser = new User
            {
                Id = "12345",
                Role = UserRole.Admin,
                Pseudo = "John"
            };
            var hotelId = Guid.NewGuid();

            _mockUserService.Setup(service => service.GetCurrentUser()).ReturnsAsync(mockCurrentUser);

            var deletedHotel = new Hotel { Id = hotelId, Name = "Deleted Hotel", Location = "Paris" };

            _mockHotelService.Setup(service => service.Delete(hotelId)).ReturnsAsync(deletedHotel);

            var result = await controller.Delete(hotelId);

            result.Should().BeOfType<ApiResponse<Hotel>>();
            var apiResponse = result as ApiResponse<Hotel>;
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().BeEquivalentTo(deletedHotel);
            apiResponse.StatusCode.Should().Be(200);
        }
    }
}
