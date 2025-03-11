using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers;
using API.Services.UserService;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;
using System.Text.Json;
using API.DTO.UserDto;

namespace API.Tests.Functional
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly ITestOutputHelper output;

        public UserControllerTest(ITestOutputHelper output)
        {
            _mockUserService = new Mock<IUserService>();
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null);
            this.output = output;
        }

        private UserController SetUpControllerWithClaims(UserRole role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "12345"),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            var controller = new UserController(_mockUserService.Object, _mockUserManager.Object);

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
        public async Task GetUsers_ShouldReturnUsers_WhenUserIsAdmin()
        {
            var controller = SetUpControllerWithClaims(UserRole.Admin);

            var mockCurrentUser = new User
            {
                Id = "12345",
                Role = UserRole.Admin,
                Pseudo = "John"
            };
            _mockUserService.Setup(service => service.GetCurrentUser()).ReturnsAsync(mockCurrentUser);

            var mockUsers = new List<User>
            {
                new User { UserName = "testuser1", Role = UserRole.Admin },
                new User { UserName = "testuser2", Role = UserRole.User }
            };

            _mockUserService.Setup(service => service.GetAll())
                            .ReturnsAsync(mockUsers);

            var result = await controller.Index();

            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(mockUsers);
            result.Message.Should().BeEmpty();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
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

            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("You cannot get users");
            result.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Register_ShouldReturnSuccess_WhenUserIsCreated()
        {
            var controller = new UserController(_mockUserService.Object, _mockUserManager.Object); // on ne veut pas tester avec un controller où l'on a mocké une authentification car il n'est pas nécessaire d'être authentifié

            var postUserDto = new PostUserDto
            {
                Email = "newuser@example.com",
                Pseudo = "newuser",
                Password = "Password123!"
            };

            var mockUser = new User
            {
                UserName = postUserDto.Email,
                Email = postUserDto.Email,
                Pseudo = postUserDto.Pseudo,
                Role = UserRole.User
            };

            var identityResult = IdentityResult.Success;

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), postUserDto.Password))
                            .ReturnsAsync(identityResult);

            var result = await controller.Register(postUserDto);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(new { message = "user created" });
        }

        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenUserUpdatesOwnInfo()
        {
            var controller = SetUpControllerWithClaims(UserRole.User);

            var userId = Guid.NewGuid();

            var mockCurrentUser = new User
            {
                Id = userId.ToString(),
                Role = UserRole.User,
                Pseudo = "John"
            };

            _mockUserService.Setup(m => m.GetCurrentUser()).ReturnsAsync(mockCurrentUser);

            var putUserDto = new PutUserDto { Pseudo = "updateduser" };

            var updatedUser = new User
            {
                Id = userId.ToString(),
                Role = UserRole.User,
                Pseudo = "updateduser"
            };

            _mockUserService.Setup(m => m.Update(userId, putUserDto)).ReturnsAsync(updatedUser);

            var result = await controller.Edit(userId, putUserDto);

            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(updatedUser);
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Update_ShouldFail_WhenUserUpdatesOtherUser()
        {
            var controller = SetUpControllerWithClaims(UserRole.User);

            var mockCurrentUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Role = UserRole.User,
                Pseudo = "John"
            };

            _mockUserService.Setup(m => m.GetCurrentUser()).ReturnsAsync(mockCurrentUser);

            var otherUserId = Guid.NewGuid();
            var otherUser = new User
            {
                Id = otherUserId.ToString(),
                Role = UserRole.User,
                Pseudo = "Jane"
            };

            var putUserDto = new PutUserDto { Pseudo = "Jane1234" };

            _mockUserService.Setup(m => m.Update(otherUserId, putUserDto)).ReturnsAsync(otherUser);

            var result = await controller.Edit(otherUserId, putUserDto);

            result.Success.Should().BeFalse();
            result.Message.Should().Be("You cannot update other user");
            result.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenUserDeletesOwnInfo()
        {
            var controller = SetUpControllerWithClaims(UserRole.User);

            var userId = Guid.NewGuid();

            var mockCurrentUser = new User
            {
                Id = userId.ToString(),
                Role = UserRole.User,
                Pseudo = "John"
            };

            _mockUserService.Setup(m => m.GetCurrentUser()).ReturnsAsync(mockCurrentUser);

            _mockUserService.Setup(m => m.Delete(userId)).ReturnsAsync(mockCurrentUser);

            var result = await controller.Delete(userId);

            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(mockCurrentUser);
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Delete_ShouldFail_WhenUserDeletesOtherUser()
        {
            var controller = SetUpControllerWithClaims(UserRole.User);

            var mockCurrentUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Role = UserRole.User,
                Pseudo = "John"
            };

            _mockUserService.Setup(m => m.GetCurrentUser()).ReturnsAsync(mockCurrentUser);

            var otherUserId = Guid.NewGuid();
            var otherUser = new User
            {
                Id = otherUserId.ToString(),
                Role = UserRole.User,
                Pseudo = "Jane"
            };

            _mockUserService.Setup(m => m.Delete(otherUserId)).ReturnsAsync(otherUser);

            var result = await controller.Delete(otherUserId);

            result.Success.Should().BeFalse();
            result.Message.Should().Be("You cannot delete other user");
            result.StatusCode.Should().Be(401);
        }
    }
}
