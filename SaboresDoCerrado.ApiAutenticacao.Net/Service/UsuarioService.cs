using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPerfilRepository _perfilRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository usuarioRepository, IPerfilRepository perfilRepository, ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _perfilRepository = perfilRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UsuarioDTO>> ObterTodosAsync()
        {
            _logger.LogInformation("Listando todos os usuarios");
            return await _usuarioRepository.ObterTodosAsync();
        }

        public async Task<UsuarioDTO?> ObterPorIdNoTrackAsync(int id)
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
        public async Task<UsuarioDTO?> UpdateUsuarioPorId(int id, UsuarioUpdateRequestDTO usuarioUpdateRequestDTO)
        {
            _logger.LogInformation("Tentando atualizar o usuario pelo id [{id}]", id);
            var perfisExistentesCount = await _perfilRepository.ContarPerfisExistentesAsync(usuarioUpdateRequestDTO.PerfilIds);
            if (perfisExistentesCount != usuarioUpdateRequestDTO.PerfilIds.Count)
            {
                _logger.LogWarning("Tentativa de registro com um ou mais IDs de perfil inválidos.");
                throw new InvalidOperationException("Um ou mais perfis fornecidos são inválidos.");
            }
            _logger.LogInformation("Tentando atualizar usuario pelo id [{id}]", id);
            return await _usuarioRepository.UpdateUsuarioPorId(id, usuarioUpdateRequestDTO);
        }

        public async Task<bool> UpdateSenhaUsuarioPorIdAsync(int Id, UsuarioUpdateSenhaRequestDTO UsuarioUpdateSenhaRequestDTO)
        {
            _logger.LogInformation("Buscando usuário [{id}]", Id);
            var Entidade = await _usuarioRepository.ObterPorIdAsync(Id);
            if (Entidade is null)
            {
                _logger.LogWarning("Tentando atualizar senha de um usuário não cadastrado na base. Usuario [{id}]", Id);
                return false;
            }
            _logger.LogInformation("Validando senha atual do usuário [{Id}]", Id);
            if(!BCrypt.Net.BCrypt.Verify(UsuarioUpdateSenhaRequestDTO.SenhaAtual, Entidade.HashSenha)){
                var msg = $"Falha na alteração de senha para o usuário ID [{Id}]: Senha atual incorreta.";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }

            _logger.LogDebug("Senha atual verificada com sucesso. Gerando novo hash para o usuário ID [{UsuarioId}]", Id);
            var novoHash = BCrypt.Net.BCrypt.HashPassword(UsuarioUpdateSenhaRequestDTO.NovaSenha);
            Entidade.HashSenha = novoHash;
            Entidade.DataAtualizacao = DateTime.UtcNow;
            _logger.LogInformation("Tentando atualizar senha usuario pelo id [{id}]", Id);
            await _usuarioRepository.UpdateSenhaUsuarioPorIdAsync(Entidade);

            _logger.LogInformation("Senha do usuário ID [{UsuarioId}] alterada com sucesso.", Id);
            return true;
        }
    }
}
