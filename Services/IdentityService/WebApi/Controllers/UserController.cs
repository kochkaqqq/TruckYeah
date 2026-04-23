using Application.Interfaces;
using Application.Shared.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDtoRequest regDto)
        {
            try
            {
                var result = await _userService.RegistrationUserAsync(regDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
