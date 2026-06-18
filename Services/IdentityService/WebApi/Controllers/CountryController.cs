using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountryController : ControllerBase
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
            return Ok(countries.Select(c => new { c.Id, Name = c.Name.Value }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountry([FromRoute] Guid id)
        {
            var country = await _countryService.GetCountry(id);
            return Ok(new { country.Id, Name = country.Name.Value });
        }

        [HttpPost]
        public async Task<IActionResult> AddResult([FromBody] string countryName)
        {
            var result = await _countryService.AddCountry(countryName);
            return Ok(new { result.Id, Name = result.Name.Value });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry([FromRoute] Guid id)
        {
            await _countryService.DeleteCiuntry(id);
            return Ok();
        }
    }
}
