using VollMed.Web.Dtos;
using VollMed.Web.Interfaces;
using VollMed.Web.Models;

namespace VollMed.Web.Services
{
    public class VollMedApiService : BaseHttpService, IVollMedApiService
    {
        class ApiUris
        {
            public static string ListarConsultas = "api/Consulta/listar";
            public static string ObterFormularioConsulta = "api/Consulta/formulario";
            public static string SalvarConsulta = "api/Consulta/Salvar";
            public static string ExcluirConsulta = "api/Consulta/Excluir";

            public static string ListarMedicos = "api/Medico/Listar";
            public static string ObterFormularioMedico = "api/Medico/formulario";
            public static string SalvarMedico = "api/Medico/Salvar";
            public static string ExcluirMedico = "api/Medico/Excluir";
            public static string ListarMedicosPorEspecialidade = "api/Medico/especialidade";
        }

        public VollMedApiService(
            IConfiguration configuration
            , IHttpClientFactory httpClientFactory
            , ILogger<BaseHttpService> logger)
            : base(configuration, httpClientFactory, logger)
        {
        }

        public IVollMedApiService WithContext(HttpContext context)
        {
            _httpContext = context;
            return this;
        }

        public async Task<PaginatedList<ConsultaDto>> ListarConsultas(int? page)
        {
            var uri = $"{ApiUris.ListarConsultas}/?page={page}";
            return await GetAsync<PaginatedList<ConsultaDto>>(uri);
        }

        public async Task<FormularioConsultaDto> ObterFormularioConsulta(long? id)
        {
            var uri = $"{ApiUris.ObterFormularioConsulta}/{id}";
            return await GetAsync<FormularioConsultaDto>(uri);
        }

        public async Task<ConsultaDto> SalvarConsulta(ConsultaDto input)
        {
            var uri = $"{ApiUris.SalvarConsulta}";
            return await PutOrPostAsync<ConsultaDto>(uri, input);
        }

        public async Task ExcluirConsulta(long consultaId)
        {
            var uri = $"{ApiUris.ExcluirConsulta}";
            await DeleteAsync<ConsultaDto>(uri, consultaId);
        }

        public async Task<PaginatedList<MedicoDto>> ListarMedicos(int? page)
        {
            var uri = $"{ApiUris.ListarMedicos}/?page={page}";
            return await GetAsync<PaginatedList<MedicoDto>>(uri);
        }

        public async Task<MedicoDto> ObterFormularioMedico(long? id)
        {
            var uri = $"{ApiUris.ObterFormularioMedico}/{id}";
            return await GetAsync<MedicoDto>(uri);
        }

        public async Task<MedicoDto> SalvarMedico(MedicoDto input)
        {
            var uri = $"{ApiUris.SalvarMedico}";
            return await PutOrPostAsync<MedicoDto>(uri, input);
        }

        public async Task ExcluirMedico(long medicoId)
        {
            var uri = $"{ApiUris.ExcluirMedico}";
            await DeleteAsync<MedicoDto>(uri, medicoId);
        }

        public async Task<IEnumerable<MedicoDto>> ListarMedicosPorEspecialidade(Especialidade especEnum)
        {
            var uri = $"{ApiUris.ListarMedicosPorEspecialidade}/{especEnum}";
            return await GetAsync<IEnumerable<MedicoDto>>(uri);
        }

        public override string Scope => _configuration["VollMed_WebApi:Scope"];
    }
}
