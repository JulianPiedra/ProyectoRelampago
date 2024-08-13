using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoRelampago.Models;
using System.Security.Claims;

namespace ProyectoRelampago.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private Context _context;
        public IndexModel(ILogger<IndexModel> logger, Context context)
        {
            _context = context;
            _logger = logger;
        }
        [BindProperty]
        public string username { get; set; }
        [BindProperty]
        public string password { get; set; }

        public IActionResult OnPost()
        {
            var user =_context.Usuarios.FirstOrDefault(u => u.Email == username && u.Contrasena == password);
            if(user != null)
            {
                TempData["Error"] = "Usuario y/o contraseña incorrectos";
                return RedirectToPage("Index");
            }
            ClaimsIdentity identity = new ClaimsIdentity("Identity.Application");
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Nombre));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(principal);
            Response.Cookies.Append("Id", user.UsuarioId.ToString());
            
            
            return RedirectToPage("ControlMarcas");
        }
    }
}
