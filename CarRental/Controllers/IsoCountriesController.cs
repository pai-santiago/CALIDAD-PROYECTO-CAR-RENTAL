using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers;

[ApiController]
[Route("[controller]")]
public class IsoCountriesController([FromKeyedServices("MySql")] ICountryRepository countryRepository) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(countryRepository.GetCountries());

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var country = countryRepository.GetCountryById(id);
        return country is null ? NotFound() : Ok(country);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Country country)
    {
        if (!countryRepository.InsertCountry(country))
            return Problem("The country could not be created.");

        return CreatedAtAction(nameof(GetById), new { id = country.IdCountry }, country);
    }

    [HttpPut]
    public IActionResult Put([FromBody] Country country)
        => countryRepository.UpdateCountry(country) ? NoContent() : NotFound();
}
