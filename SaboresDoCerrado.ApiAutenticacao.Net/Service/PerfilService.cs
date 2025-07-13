using Mapster;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public class PerfilService : IPerfilService
    {
        private readonly IPerfilRepository _perfilRepository;
        private readonly ILogger<PerfilService> _logger;


        public PerfilService(IPerfilRepository repositorioPerfil, ILogger<PerfilService> logger)
        {
            _perfilRepository = repositorioPerfil;
            _logger = logger;
        }


        public async Task<IEnumerable<PerfilDTO>> ObterTodosAsync()
        {
            _logger.LogInformation("Iniciando listagem de Perfis");
            var perfisEntidade = await _perfilRepository.ObterTodosAsync();
            return perfisEntidade.Adapt<IEnumerable<PerfilDTO>>();
        }
        public async Task<PerfilDTO> ObterPorIdAsync(int id)
        {
            var perfilEntidade = await _perfilRepository.ObterPorIdNoTrackingAsync(id);
            if (perfilEntidade == null)
            {
                return null;
            }
            var perfilDto = perfilEntidade.Adapt<PerfilDTO>();
            return perfilDto;
        }
        public async Task<PerfilDTO?> UpdatePerfilPorIdAsync(int id, UpdatePerfilRequestDTO perfilUpdateRequestDTO)
        {
            _logger.LogInformation("Tentando atualizar o perfil [{id}]", id);
            var entidade = await _perfilRepository.UpdatePorIdAsync(id, perfilUpdateRequestDTO);
            if (entidade is not null)
            {
                var perfilDto = entidade.Adapt<PerfilDTO>();
                return perfilDto;
            }
            return null;
        }
        public async Task<bool?> InativarAtivarPerfilAsync(int id, bool novoStatus)
        {
            _logger.LogInformation("Tentando atualizar o status do perfil [{id}], para o status: [{status}]", id, novoStatus);
            var sucesso = await _perfilRepository.InativarAtivarPerfilAsync(id, novoStatus);
            if (sucesso is null) _logger.LogWarning("Perfil [{id}] não encontrado!", id);
            if (sucesso.Equals(false)) _logger.LogWarning("O perfil [{id}] já está no status: [{novoStatus}]", id, novoStatus);
            return sucesso;
        }
        public async Task<PerfilDTO> CadastraPerfilAsync(PostPerfilRequestDTO perfilRequestDTO)
        {
            var perfil = new Perfil
            {
                Nome = perfilRequestDTO.Nome,
                Descricao = perfilRequestDTO.Descricao,
                Status = true
            };
            var novaEntidade = await _perfilRepository.CadastraPerfilAsync(perfil);
            return novaEntidade.Adapt(new PerfilDTO());
        }
    }
}
