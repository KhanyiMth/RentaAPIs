namespace RentaAPIs.DTOs
{
    public class LoginRequestDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? TwoFactorCode { get; set; }

    }
}
