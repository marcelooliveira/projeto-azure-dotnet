using Microsoft.AspNetCore.Mvc;
using VollMed.Web.Dtos;
using VollMed.Web.Exceptions;
using VollMed.Web.Interfaces;

namespace VollMed.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultaController : ControllerBase
    {
        private readonly IConsultaService _consultaservice;
        private readonly IMedicoService _medicoService;
     

        public ConsultaController(IConsultaService consultaService, IMedicoService medicoService)
        {
            _consultaservice = consultaService;
            _medicoService = medicoService;            
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarAsync([FromQuery] int page = 1)
        {
            PaginatedList<ConsultaDto> consultas = await _consultaservice.ListarAsync(page);
            return Ok(consultas);
        }

        [HttpGet("formulario/{id?}")]
        public async Task<IActionResult> ObterFormularioAsync(long id = 0)
        {
            var dados = id > 0
                ? await _consultaservice.CarregarPorIdAsync(id)
                : new ConsultaDto { Data = DateTime.Now };
            IEnumerable<MedicoDto> medicos = _medicoService.ListarTodos();
            var formularioConsulta = new FormularioConsultaDto
            {
                Consulta = dados,
                Medicos = medicos
            };
            return Ok(formularioConsulta);
        }

        [HttpPut("Salvar")]
        [HttpPost("Salvar")]
        public async Task<IActionResult> SalvarAsync([FromBody] ConsultaDto dados)
        {
            try
            {
                await _consultaservice.CadastrarAsync(dados);
                return Ok(dados);
            }
            catch (RegraDeNegocioException ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }

        [HttpDelete("Excluir/{id}")]
        public async Task<IActionResult> ExcluirAsync(int id)
        {
            await _consultaservice.ExcluirAsync(id);
            return Ok();
        }
    }
}
