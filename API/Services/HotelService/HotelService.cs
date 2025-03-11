using API.DTO.HotelDto;
using API.Models;
using API.Repository.HotelRepository;
using System.Text.Json;

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
        public async Task<Hotel> Create(PostHotelDto postHotelDto)
        {
            if (postHotelDto.NightPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(postHotelDto.NightPrice), "Night price cannot be negative.");
            }
            if (string.IsNullOrWhiteSpace(postHotelDto.Name))
            {
                throw new ArgumentException("Hotel name is required.", nameof(postHotelDto.Name));
            }
            if (string.IsNullOrWhiteSpace(postHotelDto.Location))
            {
                throw new ArgumentException("Hotel location is required.", nameof(postHotelDto.Location));
            }
            if (string.IsNullOrWhiteSpace(postHotelDto.Description))
            {
                throw new ArgumentException("Hotel description is required.", nameof(postHotelDto.Description));
            }
            var createdHotel = await hotelRepository.Create(postHotelDto);
            var hotelPictures = await CreateHotelBlob(postHotelDto.Pictures, createdHotel.Id);
            await hotelRepository.AddPictures(hotelPictures);
            return createdHotel;
        }
        public async Task<Hotel> Update(Guid id, PatchHotelDto patchHotelDto)
        {
            var hotel = await Get(id);
            if (hotel == null)
            {
                throw new KeyNotFoundException("Hotel not found");
            }
            if (patchHotelDto.Pictures != null)
            {
                var newHotelPictures = await CreateHotelBlob(patchHotelDto.Pictures, hotel.Id);
                await hotelRepository.AddPictures(newHotelPictures);
                hotel.PictureList = newHotelPictures;
            }

            var updatedHotel = await hotelRepository.Update(hotel, patchHotelDto);
            return updatedHotel;
        }


        public async Task<Hotel> Delete(Guid id)
        {
            var hotel = await Get(id);
            var deletedHotel = await hotelRepository.Delete(hotel);
            return deletedHotel;
        }

        public async Task<List<HotelBlob>> CreateHotelBlob(List<IFormFile> files, Guid hotelId)
        {
            var pictures = new List<HotelBlob>();

            foreach (var file in files)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var hotelBlob = new HotelBlob
                {
                    HotelId = hotelId,
                    FileName = fileName,
                    MimeType = file.ContentType,
                };
                await SaveImageAsync(file, fileName);

                pictures.Add(hotelBlob);
            }
            return pictures;
        }
        public async Task SaveImageAsync(IFormFile file, string fileName)
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "images");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
    }
}
