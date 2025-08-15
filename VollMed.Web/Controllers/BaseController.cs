using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VollMed.Web.Models;

namespace VollMed.Web.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["Especialidades"] = GetEspecialidades();
            base.OnActionExecuting(context);
        }

        private List<Especialidade> GetEspecialidades()
        {
            var especialidades = (Especialidade[])Enum.GetValues(typeof(Especialidade));
            return especialidades.ToList();
        }
    }
}
