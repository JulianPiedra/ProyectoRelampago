using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoRelampago.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoRelampago.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Context _context;

        public IndexModel(ILogger<IndexModel> logger, Context context)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public string username { get; set; }

        [BindProperty]
        public string password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Email == username && u.Contrasena == password);
            if (user == null)
            {
                TempData["Error"] = "Usuario y/o contraseña incorrectos";
                return RedirectToPage();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            Response.Cookies.Append("Id", user.UsuarioId.ToString());

            return RedirectToPage("/ControlMarcas");
        }
    }
}
