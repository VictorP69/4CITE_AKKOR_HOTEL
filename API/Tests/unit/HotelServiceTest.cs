using API.DTO.HotelDto;
using API.Models;
using API.Repository.HotelRepository;
using API.Services.HotelService;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;
using System;
using Newtonsoft.Json;

namespace API.Tests.unit
{
    public class HotelServiceTest
    {

        private readonly Mock<IHotelRepository> mockHotelRepository;
        private readonly IHotelService hotelService;
        private readonly ITestOutputHelper output;

        public HotelServiceTest(ITestOutputHelper output)
        {
            mockHotelRepository = new Mock<IHotelRepository>();
            hotelService = new HotelService(mockHotelRepository.Object);
            this.output = output;
        }

        [Fact]
        public async Task GetAll_ShouldReturnHotels()
        {
            var mockedHotels = new List<Hotel>
            {
                new Hotel { Id = Guid.NewGuid(), Name = "Hotel1", Location = "Address1", Description = "Super hotel", NightPrice = 150, PictureList = new List<HotelBlob> { } },
                new Hotel { Id = Guid.NewGuid(), Name = "Hotel2", Location = "Address2", Description = "Nice hotel", NightPrice = 50, PictureList = new List<HotelBlob> { }  }
            };

            mockHotelRepository.Setup(repo => repo.GetAll()).ReturnsAsync(mockedHotels);

            var result = await hotelService.GetAll();

            result.Should().BeEquivalentTo(mockedHotels);
        }

        [Fact]
        public async Task Get_ShouldReturnHotel_WhenHotelExists()
        {
            var hotelId = Guid.NewGuid();
            var hotel = new Hotel { Id = hotelId, Name = "Hotel1", Location = "Address1", Description = "Nice hotel", NightPrice = 50, PictureList = new List<HotelBlob> { } };

            mockHotelRepository.Setup(repo => repo.Get(hotelId)).ReturnsAsync(hotel);

            var result = await hotelService.Get(hotelId);

            result.Should().BeEquivalentTo(hotel);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_WhenHotelDoesNotExist()
        {
            var hotelId = Guid.NewGuid();

            mockHotelRepository.Setup(repo => repo.Get(hotelId)).ReturnsAsync((Hotel?)null);

            var result = await hotelService.Get(hotelId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedHotel_WithPictures()
        {
            var postHotelDto = new PostHotelDto
            {
                Name = "New Hotel",
                Location = "New Address",
                Description = "Nice hotel !!!",
                NightPrice = 50,
                Pictures = new List<IFormFile> { new Mock<IFormFile>().Object }
            };

            var createdHotel = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = "New Hotel",
                Location = "New Address",
                Description = "Nice hotel !!!",
                NightPrice = 50,
                PictureList = new List<HotelBlob> { new Mock<HotelBlob>().Object }
            };

            var hotelPictures = new List<HotelBlob>
            {
                new HotelBlob { FileName = "image1.jpg", MimeType = "image/jpeg", HotelId = createdHotel.Id }
            };

            mockHotelRepository.Setup(repo => repo.Create(postHotelDto)).ReturnsAsync(createdHotel);
            mockHotelRepository.Setup(repo => repo.AddPictures(It.IsAny<List<HotelBlob>>())).Returns(Task.CompletedTask);

            var result = await hotelService.Create(postHotelDto);

            result.Should().BeEquivalentTo(createdHotel);
            mockHotelRepository.Verify(repo => repo.AddPictures(It.IsAny<List<HotelBlob>>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldFail_NegativeNightPrice()
        {
            var postHotelDto = new PostHotelDto
            {
                Name = "Toto",
                Location = "New Address",
                Description = "Nice hotel !!!",
                NightPrice = -50,
                Pictures = new List<IFormFile> { new Mock<IFormFile>().Object }
            };
            Func<Task> action = async () => await hotelService.Create(postHotelDto);

            await action.Should().ThrowAsync<ArgumentOutOfRangeException>().WithMessage("Night price cannot be negative. (Parameter 'NightPrice')");
        }

        [Fact]
        public async Task Create_ShouldFail_IfEmptyName()
        {
            var postHotelDto = new PostHotelDto
            {
                Name = "",
                Location = "New Address",
                Description = "Nice hotel !!!",
                NightPrice = 50,
                Pictures = new List<IFormFile> { new Mock<IFormFile>().Object }
            };
            Func<Task> action = async () => await hotelService.Create(postHotelDto);

            await action.Should().ThrowAsync<ArgumentException>().WithMessage("Hotel name is required. (Parameter 'Name')");
        }
        [Fact]
        public async Task Create_ShouldFail_IfEmptyLocation()
        {
            var postHotelDto = new PostHotelDto
            {
                Name = "Hotel123",
                Location = "",
                Description = "Nice hotel !!!",
                NightPrice = 50,
                Pictures = new List<IFormFile> { new Mock<IFormFile>().Object }
            };
            Func<Task> action = async () => await hotelService.Create(postHotelDto);

            await action.Should().ThrowAsync<ArgumentException>().WithMessage("Hotel location is required. (Parameter 'Location')");
        }
        [Fact]
        public async Task Create_ShouldFail_IfEmptyDescripton()
        {
            var postHotelDto = new PostHotelDto
            {
                Name = "Hotel12334",
                Location = "New Address15",
                Description = "",
                NightPrice = 50,
                Pictures = new List<IFormFile> { new Mock<IFormFile>().Object }
            };
            Func<Task> action = async () => await hotelService.Create(postHotelDto);

            await action.Should().ThrowAsync<ArgumentException>().WithMessage("Hotel description is required. (Parameter 'Description')");
        }

        [Fact]
        public async Task Update_ShouldUpdateHotel_WhenHotelExists()
        {
            var hotelId = Guid.NewGuid();
            var existingHotel = new Hotel
            {
                Id = hotelId,
                Name = "Hotel123",
                Location = "Lyon",
                Description = "Blabla",
                NightPrice = 100,
                PictureList = new List<HotelBlob> { }
            };

            var patchDto = new PatchHotelDto
            {
                Name = "Hotel12234",
                Location = "Paris",
                Description = "Hello",
                NightPrice = 150
            };

            var updatedHotel = new Hotel
            {
                Id = hotelId,
                Name = patchDto.Name!,
                Location = patchDto.Location!,
                Description = patchDto.Description!,
                NightPrice = patchDto.NightPrice!.Value,
                PictureList = existingHotel.PictureList
            };

            mockHotelRepository.Setup(repo => repo.Get(hotelId)).ReturnsAsync(existingHotel);
            mockHotelRepository.Setup(repo => repo.Update(existingHotel, patchDto)).ReturnsAsync(updatedHotel);

            var result = await hotelService.Update(hotelId, patchDto);

            result.Should().BeEquivalentTo(updatedHotel);
            mockHotelRepository.Verify(repo => repo.Update(existingHotel, patchDto), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnNull_WhenHotelDoesNotExist()
        {
            var hotelId = Guid.NewGuid();
            var patchDto = new PatchHotelDto { Name = "New Name" };

            mockHotelRepository.Setup(repo => repo.Get(hotelId)).ReturnsAsync((Hotel?)null);

            Func<Task> action = async () => await hotelService.Update(hotelId, patchDto);

            await action.Should().ThrowAsync<KeyNotFoundException>()
                            .WithMessage("Hotel not found");
        }

        [Fact]
        public async Task Update_ShouldAddNewPictures_WhenPicturesProvided()
        {
            var hotelId = Guid.NewGuid();
            var existingHotel = new Hotel
            {
                Id = hotelId,
                Name = "Hotel",
                Location = "Location",
                Description = "Description",
                NightPrice = 100,
                PictureList = new List<HotelBlob> { }
            };

            var newPictures = new List<IFormFile> { new Mock<IFormFile>().Object };
            var patchDto = new PatchHotelDto { Pictures = newPictures };

            var newHotelBlobs = new List<HotelBlob>
            {
                new HotelBlob { FileName = "newImage.jpg", MimeType = "image/jpeg", HotelId = hotelId }
            };

            var updatedHotel = new Hotel
            {
                Id = hotelId,
                Name = existingHotel.Name,
                Location = existingHotel.Location,
                Description = existingHotel.Description,
                NightPrice = existingHotel.NightPrice,
                PictureList = newHotelBlobs
            };

            mockHotelRepository.Setup(repo => repo.Get(hotelId)).ReturnsAsync(existingHotel);
            mockHotelRepository.Setup(repo => repo.AddPictures(It.IsAny<List<HotelBlob>>())).Returns(Task.CompletedTask);
            mockHotelRepository.Setup(repo => repo.Update(existingHotel, patchDto)).ReturnsAsync(updatedHotel);

            var result = await hotelService.Update(hotelId, patchDto);

            result.PictureList.Should().BeEquivalentTo(newHotelBlobs);
            mockHotelRepository.Verify(repo => repo.AddPictures(It.IsAny<List<HotelBlob>>()), Times.Once);
        }


        [Fact]
        public async Task Delete_ShouldRemoveHotel_WhenHotelExists()
        {
            var hotelId = Guid.NewGuid();
            var hotel = new Hotel
            {
                Id = hotelId,
                Name = "HotelToDelete",
                Location = "Some Address",
                Description = "Hotel to be deleted",
                NightPrice = 100,
                PictureList = new List<HotelBlob> { }
            };

            mockHotelRepository.Setup(repo => repo.Get(hotelId)).ReturnsAsync(hotel);

            mockHotelRepository.Setup(repo => repo.Delete(hotel)).ReturnsAsync(hotel);

            var result = await hotelService.Delete(hotelId);

            result.Should().BeEquivalentTo(hotel);
        }



        [Fact]
        public async Task Delete_ShouldDoNothing_WhenHotelDoesNotExist()
        {
            var hotelId = Guid.NewGuid();

            mockHotelRepository.Setup(repo => repo.Get(hotelId)).ReturnsAsync((Hotel?)null);

            var result = await hotelService.Delete(hotelId);

            result.Should().BeNull();

        }
    }
}
