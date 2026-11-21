namespace RentaAPIs.dtos
{
    public class NotificationResponseDTO
    {
        public Guid NotificationID { get; set; }
        public Guid UserID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public string ActionUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ReadAt { get; set; }

    }
}
