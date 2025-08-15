using Microsoft.AspNetCore.Mvc;
using VollMed.Web.Dtos;
using VollMed.Web.Exceptions;
using VollMed.Web.Interfaces;
using VollMed.Web.Models;

namespace VollMed.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicoController : ControllerBase
    {
        private readonly IMedicoService _service;

        public MedicoController(IMedicoService service)
        {
            _service = service;
        }

        [HttpGet("Listar")]
        public async Task<IActionResult> ListarAsync([FromQuery] int page = 1)
        {
            var medicosCadastrados = await _service.ListarAsync(page);
     
            return Ok(medicosCadastrados);
        }

        [HttpGet("formulario/{id?}")]   
        public async Task<IActionResult> ObterFormularioAsync(long id = 0)
        {
            var dados = id > 0
                ? await _service.CarregarPorIdAsync(id)
                : new MedicoDto();

            return Ok(dados);
        }

        [HttpPut("Salvar")]
        [HttpPost("Salvar")]
        public async Task<IActionResult> SalvarAsync([FromBody] MedicoDto dados)
        {
            try
            {
                await _service.CadastrarAsync(dados);
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
            await _service.ExcluirAsync(id);
            return Ok();
        }

        [HttpGet("especialidade/{especialidade}")]       
        public async Task<IActionResult> ListarPorEspecialidadeAsync(string especialidade)
        {
            if (Enum.TryParse(especialidade, out Especialidade especEnum))
            {
                var medicos = await _service.ListarPorEspecialidadeAsync(especEnum);
                return Ok(medicos);
            }
            return BadRequest("Especialidade inválida.");
        }
    }
}
