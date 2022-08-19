using API.Dtos;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(RegisterDto model)
        {
            var result = await this._userService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("get-token")]
        public async Task<IActionResult> GetTokenAsync(LoginDto model)
        {
            var result = await this._userService.GetTokenAsync(model);
            this.SetRefreshTokenCookie(result.RefreshToken);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsyn()
        {
            var refreshToken = Request.Cookies["testRefreshToken"];
            var response = await this._userService.RefreshTokenAsync(refreshToken);
            if (!string.IsNullOrEmpty(response.RefreshToken))
            {
                this.SetRefreshTokenCookie(response.RefreshToken);
            }
            return Ok(response);
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(20)
            };
            Response.Cookies.Append("testRefreshToken", refreshToken, cookieOptions);
        }

        [HttpPost("add-role")]
        public async Task<ActionResult> AddUserRoleAsync(AddRoleDto model)
        {
            var result = await this._userService.AddRoleAsync(model);
            return Ok(result);
        }

    }
}
