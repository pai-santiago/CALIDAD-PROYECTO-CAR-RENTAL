using System.ComponentModel.DataAnnotations;

namespace CarRentalServices.Entities
{
    public class Customer
    {
        [Key]
        public int IdCustomer { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string SocialSecurityNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string MobilePhone { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
    }
}
