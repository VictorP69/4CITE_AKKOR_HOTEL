using API.Services.UserService;
using FluentAssertions;
using Xunit;
using Moq;
using API.Repository.UserRepository;
using API.Models;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using API.DTO.UserDto;
using Xunit.Abstractions;
using System.Text.Json;

namespace API.Tests.unit
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> mockUserRepository;
        private readonly Mock<IMapper> mockMapper;
        private readonly IUserService userService;
        private readonly ITestOutputHelper output;
        private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;

        public UserServiceTest(ITestOutputHelper output)
        {
            mockUserRepository = new Mock<IUserRepository>();
            mockMapper = new Mock<IMapper>();
            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            userService = new UserService(mockUserRepository.Object, mockMapper.Object, mockHttpContextAccessor.Object);
            this.output = output;
        }

        [Fact]
        public async Task GetAll_ShouldReturnUsers()
        {
            var mockedUsers = new List<User>
            {
                new User { Id = Guid.NewGuid().ToString(), Pseudo = "John Doe", Email = "john@doe.com", PasswordHash = "Password1234." },
                new User { Id = Guid.NewGuid().ToString(), Pseudo = "Jane Doe", Email = "jane@doe.com", PasswordHash = "Password12345." }
            };

            mockUserRepository.Setup(repo => repo.GetAll()).ReturnsAsync(mockedUsers);

            var result = await userService.GetAll();

            result.Should().BeEquivalentTo(mockedUsers);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyListWhenNoUsers()
        {
            var mockedUsers = new List<User>();

            mockUserRepository.Setup(repo => repo.GetAll()).ReturnsAsync(mockedUsers);

            var result = await userService.GetAll();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Get_ShouldReturnUser_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId.ToString(),
                Pseudo = "John Doe",
                Email = "john@doe.com",
                PasswordHash = "Password1234."
            };

            mockUserRepository.Setup(repo => repo.Get(userId)).ReturnsAsync(user);

            var result = await userService.Get(userId);

            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            mockUserRepository.Setup(repo => repo.Get(userId)).ReturnsAsync((User)null);

            var result = await userService.Get(userId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Update_PseudoShouldReturnUpdatedUser_IfUserExist()
        {
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                Id = userId.ToString(),
                Pseudo = "John123",
                Email = "john@doe.com",
                PasswordHash = "Password1234."
            };

            var updatedUserDto = new PutUserDto
            {
                Pseudo = "John1234"
            };

            var updatedUser = new User
            {
                Id = userId.ToString(),
                Pseudo = updatedUserDto.Pseudo,
                Email = existingUser.Email,
                PasswordHash = existingUser.PasswordHash
            };

            mockUserRepository.Setup(repo => repo.Update(userId, updatedUserDto)).ReturnsAsync(updatedUser);

            mockMapper.Setup(mapper => mapper.Map<User>(It.IsAny<object>())).Returns(updatedUser);

            var result = await userService.Update(userId, updatedUserDto);

            result.Should().BeEquivalentTo(updatedUser);
        }
        [Fact]
        public async Task Update_PseudoShouldReturnUpdatedUser_IfUserDoesntExist()
        {
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                Id = userId.ToString(),
                Pseudo = "John123",
                Email = "john@doe.com",
                PasswordHash = "Password1234."
            };

            var updatedUserDto = new PutUserDto
            {
                Pseudo = "John1234"
            };

            var updatedUser = new User
            {
                Id = userId.ToString(),
                Pseudo = updatedUserDto.Pseudo,
                Email = existingUser.Email,
                PasswordHash = existingUser.PasswordHash
            };

            mockUserRepository.Setup(repo => repo.Update(userId, updatedUserDto)).ReturnsAsync((User)null);

            mockMapper.Setup(mapper => mapper.Map<User>(It.IsAny<object>())).Returns((User)null);

            var result = await userService.Update(userId, updatedUserDto);

            result.Should().BeNull();
        }


        [Fact]
        public async Task Delete_UserShouldWork_IfUserExist()
        {
            var userId = Guid.NewGuid();
            var userToDelete = new User
            {
                Id = userId.ToString(),
                Pseudo = "John123",
                Email = "john@doe.com",
                PasswordHash = "Password1234."
            };

            mockUserRepository.Setup(repo => repo.Delete(userId)).ReturnsAsync(userToDelete);

            mockMapper.Setup(mapper => mapper.Map<User>(It.IsAny<object>())).Returns(userToDelete);

            var result = await userService.Delete(userId);

            result.Should().BeEquivalentTo(userToDelete);
        }

        [Fact]
        public async Task Delete_UserShouldWork_IfUserDoesntExist()
        {
            var userId = Guid.NewGuid();
            var userToDelete = new User
            {
                Id = userId.ToString(),
                Pseudo = "John123",
                Email = "john@doe.com",
                PasswordHash = "Password1234."
            };

            mockUserRepository.Setup(repo => repo.Delete(userId)).ReturnsAsync((User)null);

            mockMapper.Setup(mapper => mapper.Map<User>(It.IsAny<object>())).Returns((User)null);

            var result = await userService.Delete(userId);

            result.Should().BeNull();
        }
    }
}
