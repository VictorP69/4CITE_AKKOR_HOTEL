using System.Text.Json.Serialization;

namespace API.Models
{
    public class HotelBlob
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public Guid HotelId { get; set; }
        [JsonIgnore]
        public Hotel Hotel { get; set; }
    }
}
