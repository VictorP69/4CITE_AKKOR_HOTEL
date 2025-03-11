using API.DTO.BookingDto;
using API.Models;

namespace API.Services.BookingService
{
    public interface IBookingService
    {
        public Task<List<Booking>> GetAll();
        public Task<Booking> Get(Guid id);
        public Task<List<Booking>> GetByUser(Guid userId);
        public Task<Booking> Create(PostBookingDto createHotelDto);
        public Task<Booking> Update(Guid id, PutBookingDto patchBookingDto);
        public Task<Booking> Delete(Guid id);
    }
}
