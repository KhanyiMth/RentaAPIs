using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class User
    {
        public Guid UserID { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
