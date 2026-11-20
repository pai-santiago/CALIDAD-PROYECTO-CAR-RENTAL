using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IsoCountriesController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;

        public IsoCountriesController([FromKeyedServices("MySql")] ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_countryRepository.GetCountries());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var country = _countryRepository.GetCountryById(id);
            return country is null ? NotFound() : Ok(country);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Country country)
        {
            if (!_countryRepository.InsertCountry(country))
            {
                return Problem("The country could not be created.");
            }

            return CreatedAtAction(nameof(GetById), new { id = country.IdCountry }, country);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Country country)
        {
            return _countryRepository.UpdateCountry(country) ? NoContent() : NotFound();
        }
    }
}
