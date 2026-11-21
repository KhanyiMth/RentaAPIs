namespace RentaAPIs.DTOs
{
    public class UpdateUnitDTO
    {

        public int UnitNumber { get; set; }
        public int Parking { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal RentAmount { get; set; }
        public decimal Deposit { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

    }
}
