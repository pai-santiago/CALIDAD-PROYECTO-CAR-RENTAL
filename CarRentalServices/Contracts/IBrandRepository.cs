using CarRentalServices.Entities;

namespace CarRentalServices.Contracts
{
    public interface IBrandRepository
    {
        IEnumerable<Brand> GetBrands();
        Brand? GetBrandById(int id);
        bool InsertBrand(Brand brand);
        bool UpdateBrand(Brand brand);
        bool DeleteBrand(int id);
    }
}
