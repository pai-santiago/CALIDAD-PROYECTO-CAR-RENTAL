using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController([FromKeyedServices("MySql")] ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(_customerRepository.GetCustomers());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = _customerRepository.GetCustomerById(id);
            return customer is null ? NotFound() : Ok(customer);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Customer customer)
        {
            if (!_customerRepository.InsertCustomer(customer))
            {
                return Problem("The customer could not be created.");
            }

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.IdCustomer },
                customer
            );
        }

        [HttpPut]
        public IActionResult Update([FromBody] Customer customer)
        {
            return _customerRepository.UpdateCustomer(customer) ? NoContent() : NotFound();
        }
    }
}