using CarRentalServices.Entities;

namespace CarRentalServices.Contracts
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers();
        Customer? GetCustomerById(int id);
        bool InsertCustomer(Customer customer);
        bool UpdateCustomer(Customer customer);
    }
}
