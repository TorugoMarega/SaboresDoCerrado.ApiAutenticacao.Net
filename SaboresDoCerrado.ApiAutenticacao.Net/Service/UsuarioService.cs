using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UsuarioDTO>> ObterTodosAsync()
        {
            _logger.LogInformation("Listando todos os usuarios");
            return await _usuarioRepository.ObterTodosAsync();
        }

        public async Task<UsuarioDTO?> ObterPorIdAsync(int id)
        {
            _logger.LogInformation("Buscando usuario pelo id [{id}]", id);
            return await _usuarioRepository.ObterPorIdNoTrackAsync(id);
        }
        public async Task<bool> InativarAtivarUsuarioAsync(int id, bool novoStatus)
        {
            var acao = novoStatus ? "ativação" : "inativação";
            _logger.LogInformation("Executando regra de negócio para {Acao} do usuário ID: [{UsuarioId}]", acao, id);
            return await _usuarioRepository.InativarAivarUsuarioAsync(id, novoStatus);
        }
        public async Task<UsuarioDTO?> UpdateUsuarioPorId(int id, UsuarioUpdateRequestDTO usuarioUpdateRequestDTO) {
            _logger.LogInformation("Buscando usuario pelo id [{id}]", id);
            var entidade = await _usuarioRepository.ObterPorIdAsync(id);
            _logger.LogInformation("Atualizando usuario pelo id [{id}]", id);
            //adicionar a logica do update
            _logger.LogInformation("Retornando usuario pelo id [{id}]", id);
            return await _usuarioRepository.ObterPorIdNoTrackAsync(id);
        }
    }
}
