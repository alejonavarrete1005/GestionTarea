using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionTareas.Modelos;

namespace Gestion.API.Data
{
    public class GestionAPIContext : DbContext
    {
        public GestionAPIContext (DbContextOptions<GestionAPIContext> options)
            : base(options)
        {
        }

        public DbSet<GestionTareas.Modelos.Usuario> Usuarios { get; set; } = default!;
        public DbSet<GestionTareas.Modelos.Tarea> Tareas { get; set; } = default!;
        public DbSet<GestionTareas.Modelos.Proyecto> Proyectos { get; set; } = default!;
    }
}
