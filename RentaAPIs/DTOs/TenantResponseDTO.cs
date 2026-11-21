namespace RentaAPIs.dtos
{
    public class TenantResponseDTO
    {
        public Guid TenantId { get; set; }
        public Guid? LandlordID { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; } = string.Empty;
        public int AppScore { get; set; }
        public string Cellphone { get; set; }
        //public string Password { get; set; }
        public string IDNumber { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string Occupation { get; set; }
        public string EmployerName { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public int HouseholdSize { get; set; }

    }
}
