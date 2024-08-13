using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoRelampago.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        [BindProperty]
        public string username { get; set; }
        [BindProperty]
        public string password { get; set; }

        public IActionResult OnPost()
        {
            if (username == "admin" && password == "admin")
            {
                return RedirectToPage("Admin");
            }
            else
            {
                return RedirectToPage("User");
            }
        }
    }
}
