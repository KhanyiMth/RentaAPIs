using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public interface IJwtService
    {
        string GenerateToken (AppUser user, IList<string> roles);

    }
}
