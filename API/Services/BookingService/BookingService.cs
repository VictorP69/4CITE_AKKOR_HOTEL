using API.DTO.BookingDto;
using API.Models;
using API.Repository.BookingRepository;
using API.Repository.HotelRepository;

namespace API.Services.BookingService
{
    public class BookingService(IBookingRepository bookingRepository, IHotelRepository hotelRepository) : IBookingService
    {
        public async Task<List<Booking>> GetAll()
        {
            var bookings = await bookingRepository.GetAll();
            return bookings;
        }
        public async Task<Booking> Get(Guid id)
        {
            var booking = await bookingRepository.Get(id);
            if (booking == null)
            {
                throw new Exception("Cannot find this booking");
            }
            return booking;
        }
        public async Task<List<Booking>> GetByUser(Guid userId)
        {
            var bookings = await bookingRepository.GetByUser(userId);
            return bookings;
        }
        public async Task<Booking> Create(PostBookingDto postBookingDto)
        {
            var hotel = await hotelRepository.Get(postBookingDto.HotelId);
            if (hotel == null)
            {
                throw new Exception("This hotel doesn't exist");
            }

            int nights = (postBookingDto.CheckOutDate - postBookingDto.CheckInDate).Days;
            if (nights <= 0)
            {
                throw new Exception("Invalid dates booking reservation");
            }
            
            var createdBooking = await bookingRepository.Create(postBookingDto, nights);

            return createdBooking;
        }
        public async Task<Booking> Update(Guid id, PutBookingDto putBookingDto)
        {
            var booking = await bookingRepository.Get(id);
            if (booking == null)
            {
                throw new Exception("Cannot find this booking");
            }
            int nights = (putBookingDto.CheckOutDate - putBookingDto.CheckInDate).Days;
            if (nights <= 0)
            {
                throw new Exception("Invalid dates booking reservation");
            }
            var updatedBooking = await bookingRepository.Update(booking, putBookingDto);
            return updatedBooking;
        }
        public async Task<Booking> Delete(Guid id)
        {
            var booking = await bookingRepository.Get(id);
            if (booking == null)
            {
                throw new Exception("Cannot find this booking");
            }
            var deletedBooking = await bookingRepository.Delete(booking);
            return deletedBooking;
        }
    }
}
