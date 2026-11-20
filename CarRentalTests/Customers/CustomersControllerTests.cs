using CarRental.Controllers;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CarRentalTests.Customers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerRepository> _repository = new();
    private CustomersController CreateController() => new(_repository.Object);

    [Fact]
    public void Get_ReturnsCustomers()
    {
        var customers = new[] { CreateCustomer(1) };
        _repository.Setup(repository => repository.GetCustomers()).Returns(customers);

        var result = CreateController().Index();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(customers, ok.Value);
    }

    [Fact]
    public void GetById_ReturnsCustomerWhenFound()
    {
        var customer = CreateCustomer(1);
        _repository.Setup(repository => repository.GetCustomerById(1)).Returns(customer);

        var result = CreateController().GetCustomerById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(customer, ok.Value);
    }

    [Fact]
    public void Post_CreatesCustomer()
    {
        var customer = CreateCustomer(5);
        _repository.Setup(repository => repository.InsertCustomer(customer)).Returns(true);

        var result = CreateController().Create(customer);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CustomersController.GetCustomerById), created.ActionName);
        Assert.Equal(5, created.RouteValues!["id"]);
    }

    [Fact]
    public void Put_UpdatesCustomer()
    {
        var customer = CreateCustomer(1);
        _repository.Setup(repository => repository.UpdateCustomer(customer)).Returns(true);

        var result = CreateController().Update(customer);

        Assert.IsType<NoContentResult>(result);
    }

    private static Customer CreateCustomer(int id)
    {
        return new Customer
        {
            IdCustomer = id,
            FirstName = "Ana",
            LastName = "Lopez",
            SocialSecurityNumber = "SAMPLE-001",
            Email = "ana.lopez@example.test",
            CountryId = 1,
            CountryName = "Mexico",
            MobilePhone = "555-0101",
            StateName = "Quintana Roo"
        };
    }
}
