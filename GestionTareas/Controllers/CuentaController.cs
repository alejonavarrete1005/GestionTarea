using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GestionTareas.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace GestionTareas.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public CuentaController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET: /Cuenta/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Cuenta/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new LoginDTo
            {
                Email = model.Email,
                Password = model.Password
            };

            var client = _clientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
                return View(model);
            }

            var result = await JsonSerializer.DeserializeAsync<AuthResponseDto>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            HttpContext.Session.SetString("JWToken", result.Token);

            var claims = new List<Claim>
    {
            new Claim(ClaimTypes.Name, model.Email)
    };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }


        // GET: /Cuenta/Registro
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Cuenta/Registro
        [HttpPost]
        public async Task<IActionResult> Registro(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new RegisterDTo
            {
                NombreCompleto = model.NombreCompleto,
                Correo = model.Correo,
                Clave = model.Clave
            };

            var client = _clientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/register", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error al registrarse.");
                return View(model);
            }

            return RedirectToAction("Login");
        }

        // GET: /Cuenta/Logout
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("JWToken");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }

    // DTOs auxiliares
    public class AuthResponseDto
    {
        public string Token { get; set; }
    }
}

