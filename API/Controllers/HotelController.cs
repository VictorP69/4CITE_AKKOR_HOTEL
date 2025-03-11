using API.DTO.HotelDto;
using API.Models;
using API.Services.HotelService;
using API.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("hotel")]
    public class HotelController(IUserService userService, IHotelService hotelService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<ApiResponse<List<Hotel>>> Index()
        {
            try
            {
                var hotels = await hotelService.GetAll();

                return new ApiResponse<List<Hotel>> { Data = hotels };
            } catch (Exception ex)
            {
                return new ApiResponse<List<Hotel>> { Success = false, Message = ex.Message, StatusCode = 500 };
            }

        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ApiResponse<Hotel>> Details(Guid id)
        {
            try
            {
                var currentUser = userService.GetCurrentUser();
                var hotel = await hotelService.Get(id);
                return new ApiResponse<Hotel> { Data = hotel };
            } catch (Exception ex)
            {
                return new ApiResponse<Hotel> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ApiResponse<Hotel>> Create([FromForm] PostHotelDto postHotelDto)
        {
            try
            {
                Console.WriteLine("////////////////////////////////////////////////////////////////////////////////////////");
                Console.WriteLine(JsonSerializer.Serialize(postHotelDto));

                var currentUser = await userService.GetCurrentUser();
                if (currentUser.Role == UserRole.Admin)
                {
                    var createdHotel = await hotelService.Create(postHotelDto);
                    Console.WriteLine("CREATED HOTEL");
                    Console.WriteLine(JsonSerializer.Serialize(createdHotel));

                    return new ApiResponse<Hotel> { Data = createdHotel };
                }
                else
                {
                    return new ApiResponse<Hotel> { Success = false, Message = "You cannot create an hotel", StatusCode = 401 };
                }
            } catch (Exception ex)
            {
                return new ApiResponse<Hotel> { Success= false, Message = ex.Message, StatusCode = 500 };
            }

        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ApiResponse<Hotel>> Edit(Guid id, PatchHotelDto putHotelDto)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();
                if (currentUser.Role == UserRole.Admin)
                {
                    var updatedHotel = await hotelService.Update(id, putHotelDto);
                    return new ApiResponse<Hotel> { Data = updatedHotel };
                }
                else
                {
                    return new ApiResponse<Hotel> { Success = false, Message = "You cannot update an hotel", StatusCode = 401 };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Hotel> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<Hotel>> Delete(Guid id)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();
                if (currentUser.Role == UserRole.Admin)
                {
                    var deletedHotel = await hotelService.Delete(id);
                    return new ApiResponse<Hotel> { Data = deletedHotel };
                }
                else
                {
                    return new ApiResponse<Hotel> { Success = false, Message = "You cannot delete an hotel", StatusCode = 401 };
                }
            } catch (Exception ex)
            {
                return new ApiResponse<Hotel> { Success = false, Message= ex.Message ,StatusCode = 500 };
            }
        }

        [HttpGet("image/{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            try
            {
                var currentDir = Directory.GetCurrentDirectory();

                var imagesDirectory = Path.Combine(currentDir, "images");

                var imagePath = Path.Combine(imagesDirectory, imageName);

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound($"Image {imagePath} not found");
                }

                var fileBytes = System.IO.File.ReadAllBytes(imagePath);

                return new FileContentResult(fileBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
