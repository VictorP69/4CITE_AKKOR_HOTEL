namespace API.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Hotel Hotel { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
