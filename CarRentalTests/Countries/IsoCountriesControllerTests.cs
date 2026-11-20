using CarRental.Controllers;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CarRentalTests.Countries;

public class IsoCountriesControllerTests
{
    private readonly Mock<ICountryRepository> _repository = new();
    private IsoCountriesController CreateController() => new(_repository.Object);

    [Fact]
    public void Get_ReturnsCountries()
    {
        var countries = new[] { new Country { IdCountry = 1, CountryName = "Mexico" } };
        _repository.Setup(repository => repository.GetCountries()).Returns(countries);

        var result = CreateController().Get();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(countries, ok.Value);
    }

    [Fact]
    public void GetById_ReturnsCountryWhenFound()
    {
        var country = new Country { IdCountry = 1, CountryName = "Mexico" };
        _repository.Setup(repository => repository.GetCountryById(1)).Returns(country);

        var result = CreateController().GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(country, ok.Value);
    }

    [Fact]
    public void Post_CreatesCountry()
    {
        var country = new Country { IdCountry = 5, CountryName = "Mexico" };
        _repository.Setup(repository => repository.InsertCountry(country)).Returns(true);

        var result = CreateController().Post(country);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(IsoCountriesController.GetById), created.ActionName);
        Assert.Equal(5, created.RouteValues!["id"]);
    }

    [Fact]
    public void Put_UpdatesCountry()
    {
        var country = new Country { IdCountry = 1, CountryName = "Mexico" };
        _repository.Setup(repository => repository.UpdateCountry(country)).Returns(true);

        var result = CreateController().Put(country);

        Assert.IsType<NoContentResult>(result);
    }
}
