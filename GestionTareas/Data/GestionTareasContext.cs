using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionTareas.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace GestionTareas.Data
{
    public class GestionTareasContext : DbContext
    {
        public GestionTareasContext (DbContextOptions<GestionTareasContext> options)
            : base(options)
        {
        }

        public DbSet<GestionTareas.Modelos.Usuario> Usuario { get; set; } = default!;
        public DbSet<GestionTareas.Modelos.Tarea> Tarea { get; set; } = default!;
        public DbSet<GestionTareas.Modelos.Proyecto> Proyecto { get; set; } = default!;
    }
}
