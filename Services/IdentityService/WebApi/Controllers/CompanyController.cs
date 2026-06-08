using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany([FromRoute] Guid id)
        {
            var result = await _companyService.GetCompany(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany([FromQuery] Guid id)
        {
            await _companyService.DeleteCompany(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany([FromBody] string companyName)
        {
            var result = await _companyService.AddCompany(companyName);
            return Ok(result);
        }
    }
}
