using EcommerceApplication.Application.Services;
using EcommerceApplication.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(User user)
        {
            var newUser = await _authService.CreateUserAsync(user);
            return Ok(newUser);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var verifyUser = await _authService.loginAsync(email, password);
            return Ok(verifyUser);
        }

        [HttpPost]
        [Route("updatepassword")]
        public async Task<IActionResult> UpdatePassword(string email, string oldPassword, string newPassword)
        {
            var updatePasswordUser = await _authService.ChangePasswordAsync(email, oldPassword, newPassword);
            return Ok(updatePasswordUser);
        }
    }
}
