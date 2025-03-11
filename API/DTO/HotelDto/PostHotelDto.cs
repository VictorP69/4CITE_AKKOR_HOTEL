using API.Models;

namespace API.DTO.HotelDto
{
    public class PostHotelDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public double NightPrice { get; set; }
        public List<IFormFile> Pictures { get; set; }
    }
}
