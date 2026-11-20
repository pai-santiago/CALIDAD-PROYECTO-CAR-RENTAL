using System.ComponentModel.DataAnnotations;

namespace CarRentalServices.Entities;

public class Customer
{
    [Key]
    public int IdCustomer { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string SocialSecurityNumber { get; set; }
    public required string Email { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public required string MobilePhone { get; set; }
    public required string StateName { get; set; }
}
