using VollMed.Web.Dtos;
using VollMed.Web.Exceptions;
using VollMed.Web.Interfaces;
using VollMed.Web.Models;

namespace VollMed.Web.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IMedicoRepository _repository;
        private const int PageSize = 5;

        public MedicoService(IMedicoRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedList<MedicoDto>> ListarAsync(int? page)
        {
            var medicos = _repository.GetAll();
            IQueryable<MedicoDto> dtos = medicos.Select(m => new MedicoDto(m));
            return await PaginatedList<MedicoDto>.CreateAsync(dtos, page ?? 1, PageSize);
        }

        public IEnumerable<MedicoDto> ListarTodos()
        {
            var medicos = _repository.GetAll();
            return medicos.Select(m => new MedicoDto(m)).AsEnumerable();
        }

        public async Task CadastrarAsync(MedicoDto dados)
        {
            if (await _repository.IsJaCadastradoAsync(dados.Email, dados.Crm, dados.Id))
            {
                throw new RegraDeNegocioException("E-mail ou CRM já cadastrado para outro médico!");
            }

            if (dados.Id == 0)
            {
                var medico = new Medico(dados);
                await _repository.InsertAsync(medico);
            }
            else
            {
                var medico = await _repository.FindByIdAsync(dados.Id);
                if (medico == null) throw new RegraDeNegocioException("Médico não encontrado.");

                medico.AtualizarDados(dados);
                await _repository.UpdateAsync(medico);
            }
        }

        public async Task<MedicoDto> CarregarPorIdAsync(long id)
        {
            var medico = await _repository.FindByIdAsync(id);
            if (medico == null) throw new RegraDeNegocioException("Médico não encontrado.");

            return new MedicoDto(medico);
        }

        public async Task ExcluirAsync(long id)
        {
            await _repository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<MedicoDto>> ListarPorEspecialidadeAsync(Especialidade especialidade)
        {
            var medicos = await _repository.FindByEspecialidadeAsync(especialidade);
            return medicos.Select(m => new MedicoDto(m)).ToList();
        }
    }
}


