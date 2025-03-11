using API.DTO.HotelDto;
using API.Models;

namespace API.Repository.HotelRepository
{
    public interface IHotelRepository
    {
        public Task<List<Hotel>> GetAll();
        public Task<Hotel> Get(Guid id);
        public Task<Hotel> Create(PostHotelDto postHotelDto);
        public Task AddPictures(List<HotelBlob> hotelBlobs);
        public Task<Hotel> Update(Hotel hotel, PatchHotelDto patchHotelDto);
        public Task<Hotel> Delete(Hotel hotel);
    }
}
