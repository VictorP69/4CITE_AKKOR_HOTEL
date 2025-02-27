using API.DTO;
using API.Models;
using API.Services.HotelService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("hotel")]
    public class HotelController(IHotelService hotelService) : ControllerBase
    {
        [HttpGet]
        public async Task<List<Hotel>> Index()
        {
            var hotels = await hotelService.GetAll();
            return hotels;
        }
        [HttpGet("{id}")]
        public async Task<Hotel> Details(Guid id)
        {
            var hotel = await hotelService.Get(id);
            return hotel;
        }

        [HttpPost]
        public async Task<Hotel> Create([FromBody] PostHotelDto postHotelDto)
        {
            var createdHotel = await hotelService.Create(postHotelDto);
            return createdHotel;
        }

        [HttpPut("{id}")]
        public async Task<Hotel> Edit(Guid id, PutHotelDto putHotelDto)
        {
            var updatedHotel = await hotelService.Update(id, putHotelDto);
            return updatedHotel;
        }

        [HttpDelete("{id}")]
        public async Task<Hotel> Delete(Guid id)
        {
            var deletedHotel = await hotelService.Delete(id);
            return deletedHotel;
        }
    }
}
