using System.ComponentModel.DataAnnotations;

namespace CarRentalServices.Entities
{
    public class Brand
    {
        [Key]
        public int IdBrand { get; set; }

        public required string Name { get; set; }
    }
}
