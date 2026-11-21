using System.Globalization;

namespace RentaAPIs.DTOs
{
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool RequiresTwoFactorSetup { get; set; }
        public string? AccessToken { get; set; }
        public string? Message { get; set; }

    }
}
