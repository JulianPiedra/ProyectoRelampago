using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoRelampago.Models;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
namespace ProyectoRelampago.Pages
{
    public class ControlMarcasModel : PageModel
    {
        private readonly Context _context;

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
                TempData["Errores"] = "No se pudo obtener el ID de usuario desde la cookie.";
                return;
            }

            Usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
            if (Usuario == null)
            {
                TempData["Errores"] = "Usuario no encontrado.";
                return;
            }

            Usuario.HorarioNavigation = _context.Horarios.Find(Usuario.Horario);
            if (Usuario.HorarioNavigation == null)
            {
                TempData["Errores"] = "No se encontró un horario asignado a este usuario.";
                return;
            }

            // Obtener la marca de entrada y salida para hoy
            var marcaHoy = _context.Marcas
                            .FirstOrDefault(m => m.Empleado == usuarioId && m.Fecha.HasValue && m.Fecha.Value.Date == DateTime.Today);

            // Convertir la hora de entrada y salida del usuario a TimeSpan
            if (TimeSpan.TryParse(Usuario.HorarioNavigation.HoraEntrada+":00", out TimeSpan horaEntradaEsperada) &&
                    TimeSpan.TryParse(Usuario.HorarioNavigation.HoraSalida+":00", out TimeSpan horaSalidaEsperada))
                {
                    // Revisar si la entrada fue marcada tarde
                    if (string.IsNullOrEmpty(marcaHoy.HoraEntrada))
                    {
                    EnviarCorreo("No se marcó la entrada.", "Entrada sin marcar", Usuario.Email);
                     }
                    else if (TimeSpan.TryParse(marcaHoy.HoraEntrada, out TimeSpan horaEntrada) && horaEntrada > horaEntradaEsperada)
                    {
                        EnviarCorreo("Se ha marcado la salida tarde.", "Entrada tarde", Usuario.Email);
                    }

                    
                    else if (TimeSpan.TryParse(marcaHoy.HoraSalida, out TimeSpan horaSalida) && horaSalida > horaSalidaEsperada)
                    {
                        EnviarCorreo("Se ha marcado la salida tarde.", "Salida tardía", Usuario.Email);
                    }
                }
                else
                {
                    TempData["Errores"] = "Error al convertir las horas de entrada o salida.";
                }
            
        }

        private static async Task EnviarCorreo(string correo, string subject, string destinatario)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("timemastersco@gmail.com", "yvht erfl dcnr vryg"),
                EnableSsl = true,
            })
            {
                using (var mailMessage = new MailMessage
                {
                    From = new MailAddress("timemastersco@gmail.com"),
                    Subject = subject,
                    Body = correo,
                    IsBodyHtml = false,
                })
                {
                    mailMessage.To.Add(destinatario);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }



        public IActionResult OnPostMarcarEntrada()
        {
            var id = Request.Cookies["Id"];
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int usuarioId))
            {
                TempData["Errores"] = "No se pudo obtener el ID de usuario desde la cookie.";
                return RedirectToPage();
            }
            Usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
            if (Usuario == null)
            {
                TempData["Errores"] = "Usuario no encontrado.";
                return RedirectToPage();
            }
            var horario = _context.Horarios.FirstOrDefault(h => h.HorarioId == Usuario.Horario);
            if (horario == null)
            {
                TempData["Errores"] = "No se encontró un horario válido para este usuario.";
                return RedirectToPage();
            }
            var horaActual = DateTime.Now;
            if (horaActual.TimeOfDay > TimeSpan.FromHours(horario.HoraEntrada))
            {
                TempData["Errores"] = "La hora de entrada no puede ser después de la hora permitida en el horario.";
                return RedirectToPage();
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
                TempData["Errores"] = "No se pudo obtener el ID de usuario desde la cookie.";
                return RedirectToPage();
            }

            Usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
            if (Usuario == null)
            {
                TempData["Errores"] = "Usuario no encontrado.";
                return RedirectToPage();
            }

            var marcaExistente = _context.Marcas
                .FirstOrDefault(m => m.Empleado == usuarioId && m.Fecha.HasValue && m.Fecha.Value.Date == DateTime.Today);

            if (marcaExistente == null)
            {
                TempData["Errores"] = "No se ha encontrado una entrada previa para marcar la salida.";
                return RedirectToPage();
            }

            // Validar que la hora de salida sea después de la hora de entrada
            var horaActual = DateTime.Now;
            var horaEntrada = DateTime.Parse(marcaExistente.HoraEntrada);
            Usuario.HorarioNavigation = _context.Horarios.Find(Usuario.Horario);
            var horaSalida = DateTime.Parse(Usuario.HorarioNavigation.HoraSalida.ToString()+":00");
            if (horaActual < horaEntrada)
            {
                TempData["Errores"] = "La hora de salida no puede ser antes de la hora de entrada.";
                return RedirectToPage();
            }
            if (horaActual < horaSalida)
            {
                TempData["Errores"] = "Aun no se puede marcar la salida.";
                return RedirectToPage();
            }
            if (marcaExistente.HoraSalida is not null)
            {
                TempData["Errores"] = "Marca para el día de hoy ya fue registrada.";
                return RedirectToPage();
            }


            marcaExistente.HoraSalida = horaActual.ToString("HH:mm");
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Salida marcada exitosamente a las " + horaActual.ToString("HH:mm") + ".";
            return RedirectToPage();
        }
    }
}
