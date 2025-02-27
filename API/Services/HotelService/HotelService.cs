using API.DTO;
using API.Models;
using API.Repository.HotelRepository;

namespace API.Services.HotelService
{
    public class HotelService(IHotelRepository hotelRepository) : IHotelService
    {
        public async Task<List<Hotel>> GetAll()
        {
            var hotels = await hotelRepository.GetAll();
            return hotels;
        }
        public async Task<Hotel> Get(Guid id)
        {
            var hotel = await hotelRepository.Get(id);
            return hotel;
        }
        public async Task<Hotel> Create(PostHotelDto createHotelDto)
        {
            var createdHotel = await hotelRepository.Create(createHotelDto);
            return createdHotel;
        }
        public async Task<Hotel> Update(Guid id, PutHotelDto putHotelDto)
        {
            var hotel = await Get(id);
            var updatedHotel = await hotelRepository.Update(hotel, putHotelDto);
            return updatedHotel;
        }
        public async Task<Hotel> Delete(Guid id)
        {
            var hotel = await Get(id);
            var deletedHotel = await hotelRepository.Delete(hotel);
            return deletedHotel;
        }
    }
}
