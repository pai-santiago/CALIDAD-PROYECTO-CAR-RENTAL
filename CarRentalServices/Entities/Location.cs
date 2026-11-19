using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalServices.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public required string StreetName { get; set; }
        public required string StreetNumber { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string CountryName { get; set; }
        public int CountryId { get; set; }
    }
}
