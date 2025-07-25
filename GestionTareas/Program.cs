using Microsoft.AspNetCore.Authentication.Cookies;

namespace GestionTareas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ 1. Agregar autenticación basada en cookies para mantener el token JWT en sesión
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Cuenta/Login";
                    options.AccessDeniedPath = "/Cuenta/AccessDenied";
                });

            // ✅ 2. Cliente HTTP para consumir la API (puedes usarlo en servicios o controladores)
            builder.Services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001"); // Cambia si tu API usa otro puerto
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // ✅ 3. Servicios MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // ✅ AUTENTICACIÓN antes que autorización
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
