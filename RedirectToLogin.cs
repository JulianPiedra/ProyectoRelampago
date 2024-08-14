using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProyectoRelampago.Models;
using Azure;

namespace ProyectoRelampago
{
    public class RedirectToLogin : IPageFilter
    {
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userId, out int parsedUserId))
                {
                    var _context = (Context)context.HttpContext.RequestServices.GetService(typeof(Context));

                    if (_context != null)
                    {
                        var usuario = _context.Usuarios.Find(parsedUserId);

                        if (usuario != null)
                        {
                            if (!context.HttpContext.Request.Path.Value.Contains("/ControlMarcas"))
                            {
                                context.Result = new RedirectToPageResult("/ControlMarcas");
                                context.HttpContext.Response.Cookies.Append("Id", usuario.UsuarioId.ToString());

                            }

                        }
                        else
                        {
                            context.Result = new RedirectToPageResult("/Login");
                        }
                    }
                }
            }
            else if (!user.Identity.IsAuthenticated && context.HttpContext.Request.Path.Value.Contains("/ControlMarcas"))
            {
                context.Result = new RedirectToPageResult("/Login");
            }
     


        }


        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            // No action needed
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // No action needed
        }
    }
}