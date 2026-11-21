namespace RentaAPIs.DTOs
{
    public class CreateNotificationDTO
    {
        public Guid UserID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string ActionUrl { get; set; }


    }
}
