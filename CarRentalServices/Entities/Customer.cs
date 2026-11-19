using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalServices.Entities
{
    public class Customer
    {
        [Key]
        public int IdCustomer { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string SocialSecurityNumber { get; set; }

        public required string Email { get; set; }

        public required string CountryName { get; set; }
        public int CountryId { get; set; }

        public required string MobilePhone { get; set; }

        public required string StateName { get; set; }
    }
}
