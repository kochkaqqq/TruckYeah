using AdminService.Application.Admins.Commands.CreateAdmin;
using AdminService.Application.Admins.Queries.Login;
using AdminService.Application.Admins.Queries.Refresh;
using AdminService.Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminService.WebApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Registration([FromBody] CreateAdminCommand request)
        {
            try
            {
                var res = await _mediator.Send(request);
                return Ok(res);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery request)
        {
            try
            {
                var res = await _mediator.Send(request);
                return Ok(res);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshQuery request)
        {
            try
            {
                var res = await _mediator.Send(request);
                return Ok(res);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
