using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
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
            return Ok(new { result.Id, Name = result.Name.Value });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany([FromRoute] Guid id)
        {
            await _companyService.DeleteCompany(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany([FromBody] string companyName)
        {
            var result = await _companyService.AddCompany(companyName);
            return Ok(new { result.Id, Name = result.Name.Value });
        }
    }
}
