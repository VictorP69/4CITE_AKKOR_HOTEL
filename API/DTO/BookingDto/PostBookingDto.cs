namespace API.DTO.BookingDto
{
    public class PostBookingDto
    {
        public Guid HotelId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
