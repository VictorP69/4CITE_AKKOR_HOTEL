using API.Contexts;
using API.DTO.BookingDto;
using API.Models;
using API.Services.HotelService;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.BookingRepository
{
    public class BookingRepository(DataContext context) : IBookingRepository
    {
        public async Task<List<Booking>> GetAll()
        {
            var bookings = await context.Booking.ToListAsync();
            return bookings;
        }
        public async Task<Booking> Get(Guid id)
        {
            var booking = await context.Booking.FindAsync(id);
            return booking;
        }

        public async Task<List<Booking>> GetByUser(Guid userId)
        {
            var bookings = await context.Booking
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return bookings;
        }


        public async Task<Booking> Create(PostBookingDto postBookingDto, int nights)
        {
            var hotel = await context.Hotel.FindAsync(postBookingDto.HotelId);

            var booking = new Booking
            {
                HotelId = postBookingDto.HotelId,
                UserId = postBookingDto.UserId,
                CheckInDate = postBookingDto.CheckInDate,
                CheckOutDate = postBookingDto.CheckOutDate,
                TotalPrice = hotel.NightPrice * nights
            };
            await context.Booking.AddAsync(booking);
            await context.SaveChangesAsync();
            return booking;
        }
        public async Task<Booking> Update(Booking booking, PutBookingDto putBookingDto)
        {
            booking.HotelId = booking.HotelId;
            booking.Hotel = booking.Hotel;
            booking.CheckInDate = putBookingDto.CheckInDate;
            booking.CheckOutDate = putBookingDto.CheckOutDate;
            booking.TotalPrice = booking.TotalPrice;
            booking.CreatedAt = booking.CreatedAt;

            await context.SaveChangesAsync();
            return booking;
        }
        public async Task<Booking> Delete(Booking booking)
        {
            context.Booking.Remove(booking);
            await context.SaveChangesAsync();
            return booking;
        }
    }
}
