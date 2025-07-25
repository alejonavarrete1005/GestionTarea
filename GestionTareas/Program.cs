using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Gestion.APIConsumer;
using GestionTareas.Models;
using GestionTareas.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace GestionTareas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configurar los endpoints de la API
            Crud<Usuario>.EndPoint = "https://localhost:7178/api/Usuarios";
            Crud<Proyecto>.EndPoint = "https://localhost:7178/api/Proyecto";
            Crud<Tarea>.EndPoint = "https://localhost:7178/api/Tarea";

            var builder = WebApplication.CreateBuilder(args);

            // Conexión a la base de datos
            var connectionString = builder.Configuration.GetConnectionString("GestionTareasMVCContext");
            builder.Services.AddDbContext<GestionTareasMVCContext>(options =>
                options.UseSqlServer(connectionString));

            // Añadir HttpClient con BaseAddress configurado
            builder.Services.AddHttpClient("ApiClient", client =>
            {
                var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
                if (string.IsNullOrEmpty(apiBaseUrl))
                {
                    throw new InvalidOperationException("La configuración 'ApiBaseUrl' no está definida.");
                }
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            // Agregar soporte para sesiones (opcional)
            builder.Services.AddSession();

            // Configurar autenticación con cookies
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                });

            // Si quieres validar JWT en MVC directamente (opcional)
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configuración del entorno
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Middleware del pipeline
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // habilitar sesiones

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

