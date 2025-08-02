using VollMed.Web.Dtos;

namespace VollMed.Web.Interfaces
{
    public interface IConsultaService
    {
        Task CadastrarAsync(ConsultaDto dados);
        Task<ConsultaDto> CarregarPorIdAsync(long id);
        Task ExcluirAsync(long id);
        Task<PaginatedList<ConsultaDto>> ListarAsync(int? page);
    }
}