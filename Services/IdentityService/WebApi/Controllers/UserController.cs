using Application.Interfaces;
using Application.Shared.Dtos.Requests;
using Application.Shared.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Register([FromBody] UserDtoRequest regDto)
        {
            try
            {
                var result = await _userService.RegistrationUserAsync(regDto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
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
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
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
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            try
            {
                var res = await _userService.GetUserAsync(id);
                return Ok(res);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("update-user")]
        public async Task<IActionResult> UpdateUserByAdmin([FromBody] UpdateUserRequestDto requestDto)
        {
            try
            {
                var res = await _userService.UpdateUserAsync(requestDto);
                return Ok(res);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] Guid userId)
        {
            try
            {
                await _userService.DeleteUserAsync(userId);
                return Ok();    
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        [Route("all-users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var res = await _userService.GetUsersAsync();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}