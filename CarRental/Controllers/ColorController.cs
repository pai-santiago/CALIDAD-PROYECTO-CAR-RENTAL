using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ColorController : Controller
    {
        private readonly IColorRepository _colorRepository;

        public ColorController([FromKeyedServices("MySql")] IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var lstColors = _colorRepository.GetColors();
                return Json(lstColors);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Color), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Color Id!");
            }
            try
            {
                var color = _colorRepository.GetColor(id);
                return color == null ? NotFound() : Ok(color);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult Create(Color color)
        {
            if (color is null || string.IsNullOrWhiteSpace(color.Name)) return BadRequest("Invalid Color!");
            try
            {
                var result = _colorRepository.InsertColor(color);
                return result > 0 ? CreatedAtAction(nameof(GetById), new { id = result }, color) : Problem("Failed to create color.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(Color color) 
        {
            if (color is null || color.IdColor <= 0 || string.IsNullOrWhiteSpace(color.Name))
                return BadRequest("Invalid Color!");
            try
            {
                var result = _colorRepository.UpdateColor(color);
                return result ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid Color Id!");
            try
            {
                return _colorRepository.DeleteColor(id) ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }

}
