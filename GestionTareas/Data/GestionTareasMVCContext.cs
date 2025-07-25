using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionTareas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace GestionTareas.Data
{
    public class GestionTareasMVCContext : DbContext
    {
        public GestionTareasMVCContext (DbContextOptions<GestionTareasMVCContext> options)
            : base(options)
        {
        }

        public DbSet<GestionTareas.Models.Usuario> Usuario { get; set; } = default!;
        public DbSet<GestionTareas.Models.Tarea> Tarea { get; set; } = default!;
        public DbSet<GestionTareas.Models.Proyecto> Proyecto { get; set; } = default!;
    }
}
