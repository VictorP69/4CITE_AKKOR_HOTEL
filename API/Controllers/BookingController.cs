using API.DTO.BookingDto;
using API.Models;
using API.Services.BookingService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Services.UserService;

namespace API.Controllers
{
    [ApiController]
    [Route("booking")]
    public class BookingController(IBookingService bookingService, IUserService userService) : Controller
    {
        [Authorize]
        [HttpGet]
        public async Task<ApiResponse<List<Booking>>> Index()
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();

                if (currentUser.Role == UserRole.Admin)
                {
                    var bookings = await bookingService.GetAll();
                    return new ApiResponse<List<Booking>> { Data = bookings };
                } 
                else
                {
                    return new ApiResponse<List<Booking>> { Success = false, Message = "You cannot read all bookings", StatusCode = 401 };
                }
            } catch (Exception ex)
            {
                return new ApiResponse<List<Booking>> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ApiResponse<Booking>> Details(Guid id)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();
                var booking = await bookingService.Get(id);

                if (currentUser.Id == booking.UserId.ToString() || currentUser.Role == UserRole.Admin)
                {
                    return new ApiResponse<Booking> { Data = booking };
                }
                else
                {
                    return new ApiResponse<Booking> { Success = false, Message = "You can only access your booking", StatusCode = 401 };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Booking> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ApiResponse<List<Booking>>> DetailsByUser(Guid userId)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();

                if (currentUser.Id == userId.ToString() || currentUser.Role == UserRole.Admin)
                {
                    var bookings = await bookingService.GetByUser(userId);
                    return new ApiResponse<List<Booking>> { Data = bookings };
                }
                else
                {
                    return new ApiResponse<List<Booking>> { Success = false, Message = "You can only access your bookings", StatusCode = 401 };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Booking>> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ApiResponse<Booking>> Create([FromBody] PostBookingDto postBookingDto)
        {
            try
            {
                var createdBooking = await bookingService.Create(postBookingDto);
                return new ApiResponse<Booking> { Data = createdBooking };
            } catch(Exception ex)
            {
                return new ApiResponse<Booking> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ApiResponse<Booking>> Edit(Guid id, [FromBody] PutBookingDto putBookingDto)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();
                var booking = await bookingService.Get(id);

                if (currentUser.Id == booking.UserId.ToString())
                {
                    var updatedBooking = await bookingService.Update(id, putBookingDto);
                    return new ApiResponse<Booking> { Data = updatedBooking };
                }
                else
                {
                    return new ApiResponse<Booking> { Success = false, Message = "You can only update your booking", StatusCode = 401 };
                }

            } catch (Exception ex)
            {
                return new ApiResponse<Booking> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<Booking>> Delete(Guid id)
        {
            try
            {
                var currentUser = await userService.GetCurrentUser();
                var booking = await bookingService.Get(id);

                if (currentUser.Id == booking.UserId.ToString())
                {
                    var deletedBooking = await bookingService.Delete(id);
                    return new ApiResponse<Booking> { Data = deletedBooking };
                }
                else
                {
                    return new ApiResponse<Booking> { Success = false, Message = "You can only delete your booking", StatusCode = 401 };
                }
            } catch (Exception ex)
            {
                return new ApiResponse<Booking> { Success = false, Message = ex.Message, StatusCode = 500 };
            }
        }
    }
}
