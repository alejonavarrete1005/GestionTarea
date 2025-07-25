using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Gestion.APIConsumer;
using GestionTareas.Models;
using GestionTareas.Data;
namespace GestionTareas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Crud<Usuario>.EndPoint = "https://localhost:7178/api/Usuarios";
            Crud<Proyecto>.EndPoint = "https://localhost:7178/api/Proyecto";
            Crud<Tarea>.EndPoint = "https://localhost:7178/api/Tarea";

            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.

            builder.Services.AddDbContext<GestionTareasContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("GestionTareasContext")));

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
