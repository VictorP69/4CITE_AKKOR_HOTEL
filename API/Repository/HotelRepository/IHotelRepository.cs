using API.DTO;
using API.Models;

namespace API.Repository.HotelRepository
{
    public interface IHotelRepository
    {
        public Task<List<Hotel>> GetAll();
        public Task<Hotel> Get(Guid id);
        public Task<Hotel> Create(PostHotelDto createHotelDto);
        public Task<Hotel> Update(Hotel hotel, PutHotelDto putHotelDto);
        public Task<Hotel> Delete(Hotel hotel);
    }
}
