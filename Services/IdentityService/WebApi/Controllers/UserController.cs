using Application.Interfaces;
using Application.Shared.Dtos.Requests;
using Application.Shared.Exceptions;
using Domain.Exceptions;
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
            catch (ValidationException ex)
            {
                throw new NotImplementedException();
            }
            catch (ConflictException ex)
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDtoRequest loginDto)
        {
            try
            {
                var result = await _userService.LoginUserAsync(loginDto);
                return Ok(result);
            }
            catch (EntityNotFoundException)
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> TokenRefresh([FromBody] string token)
        {
            try
            {
                var result = await _userService.RefreshTokenAsync(token);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                throw new NotImplementedException();
            }
            catch (UnauthorizedException ex)
            {
                throw new NotImplementedException();
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            try
            {
                var res = await _userService.GetUserAsync(id);
                return Ok(res);
            }
            catch (EntityNotFoundException)
            {
                throw; //TODO logging, handle errors
            }
            catch
            {
                throw; //TODO logging, handle errors
            }
        }
    }
}
