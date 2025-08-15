using Microsoft.AspNetCore.Mvc;
using VollMed.Web.Dtos;
using VollMed.Web.Interfaces;
using VollMed.Web.Models;

namespace VollMed.Web.Controllers
{
    [Route("medicos")]
    public class MedicoController : BaseController
    {
        private const string PaginaListagem = "Listagem";
        private const string PaginaCadastro = "Formulario";
        private readonly IVollMedApiService _vollMedApiService;

        public MedicoController(IVollMedApiService vollMedApiService)
        : base()
        {
            _vollMedApiService = vollMedApiService;
        }

        [HttpGet]
        [Route("{page?}")]
        public async Task<IActionResult> ListarAsync([FromQuery] int page = 1)
        {
            var medicos = await _vollMedApiService
                .WithContext(HttpContext)
                .ListarMedicos(page);
            ViewBag.Consultas = medicos;
            ViewData["Url"] = "Medicos";
            return View(PaginaListagem, medicos);
        }

        [HttpGet]
        [Route("formulario/{id?}")]
        public async Task<IActionResult> ObterFormularioAsync(long? id = 0)
        {
            MedicoDto medico = await _vollMedApiService
                .WithContext(HttpContext)
                .ObterFormularioMedico(id);
            return View(PaginaCadastro, medico);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> SalvarAsync([FromForm] MedicoDto dados)
        {
            if (dados._method == "delete")
            {
                await _vollMedApiService
                    .WithContext(HttpContext)
                    .ExcluirMedico(dados.Id);
                return Redirect("/medicos");
            }

            if (!ModelState.IsValid)
            {
                PaginatedList<MedicoDto> medicos = await _vollMedApiService
                    .WithContext(HttpContext)
                    .ListarMedicos(1);
                ViewData["Medicos"] = medicos.Items.ToList();
                return View(PaginaCadastro, dados);
            }

            try
            {
                await _vollMedApiService
                    .WithContext(HttpContext)
                    .SalvarMedico(dados);
                return Redirect("/medicos");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = ex.Message;
                ViewBag.Dados = dados;
                return View(PaginaCadastro);
            }
        }

        [HttpGet]
        [Route("especialidade/{especialidade}")]
        public async Task<IActionResult> ListarPorEspecialidadeAsync(string especialidade)
        {
            if (Enum.TryParse(especialidade, out Especialidade especEnum))
            {
                var medicos = await _vollMedApiService
                    .WithContext(HttpContext)
                    .ListarMedicosPorEspecialidade(especEnum);
                return Json(medicos);
            }
            return BadRequest("Especialidade inválida.");
        }
    }
}
