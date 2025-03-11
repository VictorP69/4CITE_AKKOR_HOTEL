using API.DTO.BookingDto;
using API.DTO.HotelDto;
using API.Models;

namespace API.Repository.BookingRepository
{
    public interface IBookingRepository
    {
        public Task<List<Booking>> GetAll();
        public Task<Booking> Get(Guid id);
        public Task<List<Booking>> GetByUser(Guid userId);
        public Task<Booking> Create(PostBookingDto postHotelDto, int nights);
        public Task<Booking> Update(Booking booking, PutBookingDto putBookingDto);
        public Task<Booking> Delete(Booking booking);
    }
}
