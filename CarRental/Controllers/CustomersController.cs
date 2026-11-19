using Azure.Core.GeoJson;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private ICustomerRepository _customerRepository;

        public CustomersController([FromKeyedServices("MySql")] ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var lstCustomers = _customerRepository.GetCustomers();
            return Json(lstCustomers);
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            if (id <= 0) return BadRequest("Invalid Customer Id!");
            var customer = _customerRepository.GetCustomerById(id);
            return customer is null ? NotFound() : Ok(customer);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Customer customer)
        {
            if (customer is null) return BadRequest("Invalid Customer!");
            var result = _customerRepository.InsertCustomer(customer);
            if (!result) return Problem("Failed to create customer.");
            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.IdCustomer },
                customer
            );
        }

        [HttpPut]
        public IActionResult Update([FromBody] Customer customer)
        {
            if (customer is null || customer.IdCustomer <= 0) return BadRequest("Invalid Customer!");
            var result = _customerRepository.UpdateCustomer(customer);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid Customer Id!");
            return _customerRepository.DeleteCustomer(id) ? NoContent() : NotFound();
        }
    }
}
