using AuthServerKeyCloak.Api.Models;
using AuthServerKeyCloak.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthServerKeyCloak.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IKeycloakServices _keycloakServices;

        public AuthController(IKeycloakServices keycloakServices)
        {
            _keycloakServices = keycloakServices;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            var response = await _keycloakServices.GetTokenAsync(model);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterModel model)
        {
            await _keycloakServices.CreateUserAsync(model);
            return Ok();
        }
    }
}
