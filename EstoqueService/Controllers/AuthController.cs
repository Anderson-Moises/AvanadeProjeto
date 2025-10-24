using Common.Jwt;
using Common.SharedModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EstoqueService.Controllers
{
    #if ESTOQUE
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(IOptions<JwtSettings> options)
        {
            _jwtService = new JwtService(options);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            if (user.Username == "admin" && user.Password == "123456")
            {
                var token = _jwtService.GenerateToken(user.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized("Usuário ou senha inválidos");
        }
    }
    #endif
}
