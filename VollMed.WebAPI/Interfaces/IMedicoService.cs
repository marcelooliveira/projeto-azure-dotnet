using VollMed.Web.Dtos;
using VollMed.Web.Models;

namespace VollMed.Web.Interfaces
{
    public interface IMedicoService
    {
        Task CadastrarAsync(MedicoDto dados);
        Task<MedicoDto> CarregarPorIdAsync(long id);
        Task ExcluirAsync(long id);
        Task<PaginatedList<MedicoDto>> ListarAsync(int? page);
        IEnumerable<MedicoDto> ListarTodos();
        Task<IEnumerable<MedicoDto>> ListarPorEspecialidadeAsync(Especialidade especialidade);

    }
}