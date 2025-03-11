using Microsoft.AspNetCore.Mvc;
using API.Services.UserService;
using API.Models;
using API.DTO.UserDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController(IUserService userService, UserManager<User> userManager) : Controller
    {
        [Authorize]
        [HttpGet]
        public async Task<ApiResponse<List<User>>> Index()
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();

                if (currentUser.Role == UserRole.Admin)
                {
                    var users = await userService.GetAll();
                    return new ApiResponse<List<User>> { Data = users };
                }
                else
                {
                    return new ApiResponse<List<User>> { Success = false, Message = "You cannot get users", StatusCode = 401 };
                }

            } catch (Exception ex)
            {
                return new ApiResponse<List<User>> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ApiResponse<User>> Details(Guid id)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();

                if (id.ToString() == currentUser.Id || currentUser.Role == UserRole.Admin)
                {
                    var user = await userService.Get(id);
                    return new ApiResponse<User> { Data = user };
                }
                else
                {
                    return new ApiResponse<User> { Success = false, Message = "You cannot get other user informations", StatusCode = 401 };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<User> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpGet("email/{email}")]
        public async Task<ApiResponse<User>> DetailsByEmail(string email)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();

                if (email == currentUser.Email || currentUser.Role == UserRole.Admin)
                {
                    var user = await userService.GetByEmail(email);
                    return new ApiResponse<User> { Data = user };
                }
                else
                {
                    return new ApiResponse<User> { Success = false, Message = "You cannot get other user informations", StatusCode = 401 };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<User> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] PostUserDto postUserDto)
        {
            var user = new User
            {
                UserName = postUserDto.Email,
                Email = postUserDto.Email,
                Pseudo = postUserDto.Pseudo,
                Role = UserRole.User
            };

            var result = await userManager.CreateAsync(user, postUserDto.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "user created" });
            }

            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ApiResponse<User>> Edit(Guid id, [FromBody] PutUserDto putUserDto)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();

                if (id.ToString() == currentUser.Id || currentUser.Role == UserRole.Admin)
                {
                    var updatedUser = await userService.Update(id, putUserDto);
                    return new ApiResponse<User> { Data = updatedUser };
                }
                else
                {
                    return new ApiResponse<User> { Success = false, Message = "You cannot update other user", StatusCode = 401 };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<User> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<User>> Delete(Guid id)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();

                if (id.ToString() == currentUser.Id)
                {
                    var updatedUser = await userService.Delete(id);
                    return new ApiResponse<User> { Data = updatedUser };
                }
                else
                {
                    return new ApiResponse<User> { Success = false, Message = "You cannot delete other user", StatusCode = 401 };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<User> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }
    }
}
