using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoRelampago.Models;
using System.Text.RegularExpressions;
namespace ProyectoRelampago.Pages
{
    public class ControlMarcasModel : PageModel
    {
        private readonly Context _context;

        public string? Errores { get; set; }
        public ControlMarcasModel(Context context)
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

            if (Usuario.HorarioNavigation == null)
            {
                Errores = "No se encontró un horario asignado a este usuario.";
            }
        }


        public IActionResult OnPostMarcarEntrada()
        {
            var id = Request.Cookies["Id"];
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int usuarioId))
            {
                Errores = "No se pudo obtener el ID de usuario desde la cookie.";
                return Page();
            }
            Usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
            if (Usuario == null)
            {
                Errores = "Usuario no encontrado.";
                return Page();
            }
            var horario = _context.Horarios.FirstOrDefault(h => h.HorarioId == Usuario.Horario);
            if (horario == null)
            {
                Errores = "No se encontró un horario válido para este usuario.";
                return Page();
            }
            var horaActual = DateTime.Now;
            if (horaActual.TimeOfDay > TimeSpan.FromHours(horario.HoraEntrada))
            {
                Errores = "La hora de entrada no puede ser después de la hora permitida en el horario.";
                return Page();
            }
            Marca = new Marcas
            {
                Empleado = usuarioId,
                Fecha = DateTime.Now,
                HoraEntrada = horaActual.ToString("HH:mm")
            };

            _context.Marcas.Add(Marca);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Entrada marcada exitosamente a las " + horaActual.ToString("HH:mm") + ".";
            return RedirectToPage();

        }

        public IActionResult OnPostMarcarSalida()
        {
            // Obtener el ID del usuario desde la cookie
            var id = Request.Cookies["Id"];
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int usuarioId))
            {
                Errores = "No se pudo obtener el ID de usuario desde la cookie.";
                return Page();
            }

            Usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
            if (Usuario == null)
            {
                Errores = "Usuario no encontrado.";
                return Page();
            }

            var marcaExistente = _context.Marcas
                .FirstOrDefault(m => m.Empleado == usuarioId && m.Fecha.HasValue && m.Fecha.Value.Date == DateTime.Today);

            if (marcaExistente == null)
            {
                Errores = "No se ha encontrado una entrada previa para marcar la salida.";
                return Page();
            }

            // Validar que la hora de salida sea después de la hora de entrada
            var horaActual = DateTime.Now;
            var horaEntrada = DateTime.Parse(marcaExistente.HoraEntrada);

            if (horaActual < horaEntrada)
            {
                Errores = "La hora de salida no puede ser antes de la hora de entrada.";
                return Page();
            }

            marcaExistente.HoraSalida = horaActual.ToString("HH:mm");
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Salida marcada exitosamente a las " + horaActual.ToString("HH:mm") + ".";
            return RedirectToPage();
        }
    }
}
