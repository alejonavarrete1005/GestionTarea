using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionTareas.Data;
using Microsoft.AspNetCore.Authorization;
using GestionTareas.Models;

namespace GestionTareas.MVC.Controllers
{
    public class TareasController : Controller
    {
        private readonly GestionTareasMVCContext _context;

        public TareasController(GestionTareasMVCContext context)
        {
            _context = context;
        }

        // GET: Tareas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tarea.ToListAsync());
        }

        // GET: Tareas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tarea = await _context.Tarea.FirstOrDefaultAsync(t => t.Id == id);

            if (tarea == null)
                return NotFound();

            return View(tarea);
        }

        // GET: Tareas/Create
       
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descripcion,Estado,Prioridad,FechaVencimiento,ProyectoId,UsuarioAsignadoId")] Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tarea);
        }

        // GET: Tareas/Edit/5
        [Authorize(Roles = "admins")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tarea = await _context.Tarea.FindAsync(id);
            if (tarea == null)
                return NotFound();

            return View(tarea);
        }

        // POST: Tareas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descripcion,Estado,Prioridad,FechaVencimiento,ProyectoId,UsuarioAsignadoId")] Tarea tarea)
        {
            if (id != tarea.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TareaExists(tarea.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tarea);
        }

        // GET: Tareas/Delete/5
        [Authorize(Roles = "admins")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tarea = await _context.Tarea.FirstOrDefaultAsync(t => t.Id == id);
            if (tarea == null)
                return NotFound();

            return View(tarea);
        }

        // POST: Tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarea = await _context.Tarea.FindAsync(id);
            if (tarea != null)
            {
                _context.Tarea.Remove(tarea);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TareaExists(int id)
        {
            return _context.Tarea.Any(e => e.Id == id);
        }
    }
}
