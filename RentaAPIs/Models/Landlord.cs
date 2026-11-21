using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class Landlord
    {
        [Key]
        public Guid LandlordID { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        //public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IDNumber { get; set; }
        public string Address { get; set; } = string.Empty;

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
