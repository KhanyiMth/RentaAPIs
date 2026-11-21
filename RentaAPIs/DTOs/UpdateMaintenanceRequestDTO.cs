using Microsoft.AspNetCore.Mvc.Formatters;

namespace RentaAPIs.DTOs
{
    public class UpdateMaintenanceRequestDTO
    {
        public string? Priority { get; set; }
        public string? Status { get; set; }

    }
}
