namespace RentaAPIs.Models
{
    public class Notification
    {
        public Guid NotificationID { get; set; }
        public Guid UserID { get; set; }
        public Guid LandlordID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; } = false;
        public string ActionUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ReadAt { get; set; }
        
        public Landlord Landlord { get; set; }

    }
}
