using API.Models;
using API.Contexts;
using Microsoft.EntityFrameworkCore;
using API.DTO.HotelDto;

namespace API.Repository.HotelRepository
{
    public class HotelRepository(DataContext context) : IHotelRepository
    {
        public async Task<List<Hotel>> GetAll()
        {
            var hotels = await context.Hotel.Include(h => h.PictureList).ToListAsync();
            return hotels;
        }
        public async Task<Hotel> Get(Guid id)
        {
            var hotel = await context.Hotel.FindAsync(id);
            return hotel;
        }

        public async Task<Hotel> Create(PostHotelDto postHotelDto)
        {
            var newHotel = new Hotel 
            {
                Name = postHotelDto.Name,
                Location = postHotelDto.Location,
                NightPrice = postHotelDto.NightPrice,
                Description = postHotelDto.Description,
            };
            await context.Hotel.AddAsync(newHotel);
            await context.SaveChangesAsync();
            return newHotel;
        }
        public async Task AddPictures(List<HotelBlob> hotelBlobs)
        {
            await context.HotelBlob.AddRangeAsync(hotelBlobs);
            await context.SaveChangesAsync();
        }

        public async Task<Hotel> Update(Hotel hotel, PatchHotelDto patchHotelDto)
        {
            if (patchHotelDto.Name != null)
                hotel.Name = patchHotelDto.Name;

            if (patchHotelDto.Location != null)
                hotel.Location = patchHotelDto.Location;

            if (patchHotelDto.Description != null)
                hotel.Description = patchHotelDto.Description;

            if (patchHotelDto.NightPrice != null)
                hotel.NightPrice = patchHotelDto.NightPrice.Value;

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
