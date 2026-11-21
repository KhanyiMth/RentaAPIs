namespace RentaAPIs.DTOs
{
    public class CreateTenantDTO
    {

        public Guid? LandlordID { get; set; } = null;
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        public string IDNumber { get; set; }
        public string Password { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string Occupation { get; set; }
        public string EmployerName { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public int HouseholdSize { get; set; }

    }
}
