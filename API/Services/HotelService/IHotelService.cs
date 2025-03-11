using API.DTO.HotelDto;
using API.Models;

namespace API.Services.HotelService
{
    public interface IHotelService
    {
        public Task<List<Hotel>> GetAll();
        public Task<Hotel> Get(Guid id);
        public Task<Hotel> Create(PostHotelDto createHotelDto);
        public Task<Hotel> Update(Guid id, PatchHotelDto patchHotelDto);
        public Task<Hotel> Delete(Guid id);
    }
}
