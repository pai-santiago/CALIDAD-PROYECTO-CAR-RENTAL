using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandRepository _brandRepository;

        public BrandsController([FromKeyedServices("MySql")] IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_brandRepository.GetBrands());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid Brand Id!");
            try
            {
                var brand = _brandRepository.GetBrandById(id);
                return brand is null ? NotFound() : Ok(brand);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Brand brand)
        {
            if (brand is null || string.IsNullOrWhiteSpace(brand.Name)) return BadRequest("Invalid Brand!");
            try
            {
                if (!_brandRepository.InsertBrand(brand)) return Problem("Failed to create brand.");
                return CreatedAtAction(nameof(GetById), new { id = brand.IdBrand }, brand);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update([FromBody] Brand brand)
        {
            if (brand is null || brand.IdBrand <= 0 || string.IsNullOrWhiteSpace(brand.Name))
                return BadRequest("Invalid Brand!");
            try
            {
                return _brandRepository.UpdateBrand(brand) ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid Brand Id!");
            try
            {
                return _brandRepository.DeleteBrand(id) ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
