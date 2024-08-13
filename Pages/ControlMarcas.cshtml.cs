using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoRelampago.Models;
using System.Text.RegularExpressions;
namespace ProyectoRelampago.Pages
{
    public class ControlMarcasModel : PageModel
    {
        private readonly Tiusr21plProyectoRelampagoJuliTamContext _context;

        public string? Errores { get; set; }
        public ControlMarcasModel(Tiusr21plProyectoRelampagoJuliTamContext context)
        {
            _context = context;
        }

        public Usuarios Usuario { get; set; }

        [BindProperty]
        public Marcas Marca { get; set; }

        public void OnGet()
        {
            // Obtener el ID del usuario desde la cookie
            var id = Request.Cookies["Id"];
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int usuarioId))
            {
                Errores = "No se pudo obtener el ID de usuario desde la cookie.";
                return;
            }

            Usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
            if (Usuario == null)
            {
                Errores = "Usuario no encontrado.";
                return;
            }
        }


        public IActionResult OnPostMarcarEntrada()
        {
            if (UsuarioIDSeleccionado == 0)
            {
                Errores = "Por favor, selecciona un usuario.";
                return Page();
            }

            Marca = new Marcas
            {
                Empleado = UsuarioIDSeleccionado,
                Fecha = DateTime.Now,
                HoraEntrada = DateTime.Now.ToString("HH:mm")
            };

            _context.Marcas.Add(Marca);
            _context.SaveChanges();

            return RedirectToPage(new { usuarioId = UsuarioIDSeleccionado });
        }

        public IActionResult OnPostMarcarSalida()
        {
            if (UsuarioIDSeleccionado == 0)
            {
                Errores = "Por favor, selecciona un usuario.";
                return Page();
            }

            var marcaExistente = _context.Marcas
                .FirstOrDefault(m => m.Empleado == UsuarioIDSeleccionado && m.Fecha.HasValue && m.Fecha.Value.Date == DateTime.Today);

            if (marcaExistente != null)
            {
                marcaExistente.HoraSalida = DateTime.Now.ToString("HH:mm");
                _context.SaveChanges();
            }

            return RedirectToPage(new { usuarioId = UsuarioIDSeleccionado });
        }
    }
}
