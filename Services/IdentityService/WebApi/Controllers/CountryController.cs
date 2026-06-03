using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _countryService.GetAllCountries();
            return Ok(countries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountry([FromQuery] Guid id)
        {
            var country = await _countryService.GetCountry(id);
            return Ok(country);
        }

        [HttpPost]
        public async Task<IActionResult> AddResult([FromBody] string countryName)
        {
            var result = await _countryService.AddCountry(countryName);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry([FromQuery] Guid id)
        {
            await _countryService.DeleteCiuntry(id);
            return Ok();
        }
    }
}
