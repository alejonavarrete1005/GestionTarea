﻿namespace GestionTareas.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; } 
        public string Descripcion { get; set; } 
        public string Estado { get; set; } 
        public string Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; } 
        public DateTime FechaVencimiento { get; set; }
    }
}
