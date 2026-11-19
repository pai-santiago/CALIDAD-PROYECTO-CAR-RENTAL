using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationsController : Controller
    {
        private readonly ILocationRepository _locationRepository;

        public LocationsController([FromKeyedServices("MySql")] ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var lstLocations = _locationRepository.GetLocations();
            return Json(lstLocations);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid Location Id!");
            var location = _locationRepository.GetLocationById(id);
            return location is null ? NotFound() : Ok(location);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Location location)
        {
            if (location is null) return BadRequest("Invalid Location!");
            var result = _locationRepository.InsertLocation(location);
            if (!result) return Problem("Failed to create location.");
            return CreatedAtAction(
                nameof(GetById),
                new { id = location.Id },
                location
            );

        }

        [HttpPut]
        public IActionResult Update([FromBody] Location location)
        {
            if (location is null || location.Id <= 0) return BadRequest("Invalid Location!");
            var result = _locationRepository.UpdateLocation(location);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid Location Id!");
            return _locationRepository.DeleteLocation(id) ? NoContent() : NotFound();
        }
    }
}
