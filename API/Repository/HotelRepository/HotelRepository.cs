using API.DTO;
using API.Models;
using API.Contexts;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.HotelRepository
{
    public class HotelRepository(DataContext context) : IHotelRepository
    {
        public async Task<List<Hotel>> GetAll()
        {
            var hotels = await context.Hotel.ToListAsync();
            return hotels;
        }
        public async Task<Hotel> Get(Guid id)
        {
            var hotel = await context.Hotel.FindAsync(id);
            return hotel;
        }

        public async Task<Hotel> Create(PostHotelDto createHotelDto)
        {
            var newHotel = new Hotel 
            {
                Name = createHotelDto.Name,
                Location = createHotelDto.Location,
                Description = createHotelDto.Description,
                PictureList = createHotelDto.PictureList
            };
            await context.Hotel.AddAsync(newHotel);
            await context.SaveChangesAsync();
            return newHotel;
        }
        public async Task<Hotel> Update(Hotel hotel, PutHotelDto putHotelDto)
        {
            hotel.Name = putHotelDto.Name;
            hotel.Location = putHotelDto.Location;
            hotel.Description = putHotelDto.Description;
            hotel.PictureList = putHotelDto.PictureList;
            await context.SaveChangesAsync();
            return hotel;
        }

        public async Task<Hotel> Delete(Hotel hotel)
        {
            context.Hotel.Remove(hotel);
            await context.SaveChangesAsync();
            return hotel;
        }
    }
}
