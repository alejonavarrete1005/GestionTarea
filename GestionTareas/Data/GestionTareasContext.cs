using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionTareas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace GestionTareas.Data
{
    public class GestionTareasContext : DbContext
    {
        public GestionTareasContext (DbContextOptions<GestionTareasContext> options)
            : base(options)
        {
        }

        public DbSet<GestionTareas.Models.Usuario> Usuarios { get; set; } = default!;
        public DbSet<GestionTareas.Models.Tarea> Tareas { get; set; } = default!;
        public DbSet<GestionTareas.Models.Proyecto> Proyectos { get; set; } = default!;
    }
}
