using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace CoworkingGestion.Filters // Ajusta el namespace según tu proyecto
{
    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _role;

        public CustomAuthorizeAttribute(string role = null)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Obtén el rol del usuario desde la sesión
            var userRole = context.HttpContext.Session.GetString("UserRole") ?? "Miembro";

            // Si se especifica un rol y el usuario no lo posee, deniega el acceso.
            if (!string.IsNullOrEmpty(_role) && userRole != _role)
            {
                context.Result = new ContentResult { Content = "Acceso denegado. No tienes permisos suficientes." };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
