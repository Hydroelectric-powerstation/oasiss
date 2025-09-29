namespace HotelWebsite.Models
{
    public class BookingRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string Checkin { get; set; } = string.Empty;
        public string Checkout { get; set; } = string.Empty;
        public int Guests { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}