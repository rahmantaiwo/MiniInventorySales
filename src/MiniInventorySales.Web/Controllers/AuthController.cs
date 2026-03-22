using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniInventorySales.Application.DTOs.Auth;
using MiniInventorySales.Application.Interface;
using System.Security.Claims;

namespace MiniInventorySales.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        [HttpGet]
        public IActionResult Login() => View(new LoginDto());

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _auth.LoginAsync(dto, ct);
            if (!result.Success || result.Data is null)
            {
                ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? result.Message);
                return View(dto);
            }

            // Thin controller: sign-in only (framework concern)
            var session = result.Data;
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
            new Claim(ClaimTypes.Name, session.FirstName),
            new Claim(ClaimTypes.Name, session.LastName),
            new Claim(ClaimTypes.Email, session.Email),
            new Claim(ClaimTypes.Role, session.Role)
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterDto());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _auth.RegisterAsync(dto, ct);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? result.Message);
                return View(dto);
            }

            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied() => View();
    }
}