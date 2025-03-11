using API.Models;

namespace API.DTO.HotelDto
{
    public class PatchHotelDto
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public double? NightPrice { get; set; }
        public List<IFormFile>? Pictures { get; set; }
    }
}
