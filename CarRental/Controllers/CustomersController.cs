using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController([FromKeyedServices("MySql")] ICustomerRepository customerRepository) : ControllerBase
{
    [HttpGet]
    public IActionResult Index() => Ok(customerRepository.GetCustomers());

    [HttpGet("{id:int}")]
    public IActionResult GetCustomerById(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid Customer Id.");

        var customer = customerRepository.GetCustomerById(id);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Customer customer)
    {
        if (customer is null)
            return BadRequest("Invalid Customer.");

        if (!customerRepository.InsertCustomer(customer))
            return Problem("The customer could not be created.");

        return CreatedAtAction(nameof(GetCustomerById), new { id = customer.IdCustomer }, customer);
    }

    [HttpPut]
    public IActionResult Update([FromBody] Customer customer)
    {
        if (customer is null || customer.IdCustomer <= 0)
            return BadRequest("Invalid Customer.");

        return customerRepository.UpdateCustomer(customer) ? NoContent() : NotFound();
    }
}
