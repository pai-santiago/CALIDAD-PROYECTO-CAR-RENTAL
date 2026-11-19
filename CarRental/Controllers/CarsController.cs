using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarsController : Controller
    {
        private readonly ICarRepository _carRepository;

        public CarsController([FromKeyedServices("MySql")] ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var lstCars = _carRepository.GetCars();
            return Json(lstCars);
        }

        [HttpGet("{id}")]
        public IActionResult GetCarById(int id)
        {
            if (id <= 0) return BadRequest("Invalid Car Id!");
            var car = _carRepository.GetCarById(id);
            return car is null ? NotFound() : Ok(car);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Car car)
        {
            if (car is null) return BadRequest("Invalid Car!");
            var result = _carRepository.InsertCar(car);
            if (!result) return Problem("Failed to create car.");
            return CreatedAtAction(
                nameof(GetCarById), // Name of the GET action method
                new { id = car.IdCar }, // Route values for the GET method
                car // The newly created object to return in the body
            );
        }

        [HttpPut]
        public IActionResult Update([FromBody] Car car)
        {
            if (car is null || car.IdCar <= 0) return BadRequest("Invalid Car!");
            var result = _carRepository.UpdateCar(car);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid Car Id!");
            return _carRepository.DeleteCar(id) ? NoContent() : NotFound();
        }
    }
}
