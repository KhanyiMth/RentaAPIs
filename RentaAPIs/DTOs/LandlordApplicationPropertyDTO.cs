using RentaAPIs.dtos;

namespace RentaAPIs.DTOs
{
    public class LandlordApplicationPropertyDTO
    {
        public Guid AdvertisementId { get; set; }

        public Guid PropertyId { get; set; }

        public Guid? UnitId { get; set; }

        public string PropertyTitle { get; set; } = string.Empty;

        public string PropertyAddress { get; set; } = string.Empty;

        public string? UnitName { get; set; }

        public decimal RentAmount { get; set; }

        public string? ImageUrl { get; set; }

        public string? Status { get; set; }

        public int ApplicationCount { get; set; }

        public int PendingApplicationCount { get; set; }

        public List<TenantApplicationResponseDTO> Applications { get; set; } = new List<TenantApplicationResponseDTO>();
    }
}
