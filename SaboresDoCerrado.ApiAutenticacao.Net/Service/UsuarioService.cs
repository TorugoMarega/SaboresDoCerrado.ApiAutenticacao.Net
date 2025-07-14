using Mapster;
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
            return await _usuarioRepository.ObterTodosUsuariosDtoAsync();
        }

        public async Task<UsuarioDTO?> ObterPorIdNoTrackAsync(int id)
        {
            _logger.LogInformation("Buscando usuario pelo id [{id}]", id);
            return await _usuarioRepository.ObterUsuarioDtoPorIdAsync(id);
        }
        public async Task<bool> InativarAtivarUsuarioAsync(int id, bool novoStatus)
        {
            var acao = novoStatus ? "ativação" : "inativação";
            _logger.LogInformation("Executando regra de negócio para {Acao} do usuário ID: [{UsuarioId}]", acao, id);
            return await _usuarioRepository.InativarAivarUsuarioAsync(id, novoStatus);
        }
        public async Task<UsuarioDTO?> AdminAtualizarUsuarioAsync(int id, AdminUsuarioUpdateRequestDTO adminUsuarioUpdateRequestDTO)
        {
            _logger.LogInformation("Admin: Iniciando processo de atualização para o usuário ID: [{UsuarioId}]", id);

            var entidade = await _usuarioRepository.ObterUsuarioEntidadePorIdAsync(id);
            if (entidade is null)
            {
                _logger.LogWarning("Admin: Tentativa de atualizar usuário não existente. ID: [{UsuarioId}]", id);
                return null;
            }

            if (await _usuarioRepository.EmailExistsInAnotherUserAsync(id, adminUsuarioUpdateRequestDTO.Email))
            {
                _logger.LogWarning("Admin: Tentativa de atualizar usuário ID [{UsuarioId}] com e-mail [{Email}] que já está em uso.", id, adminUsuarioUpdateRequestDTO.Email);
                throw new InvalidOperationException("O e-mail fornecido já está em uso por outro usuário.");
            }

            adminUsuarioUpdateRequestDTO.Adapt(entidade);
            entidade.DataAtualizacao = DateTime.UtcNow;
            entidade.UsuarioPerfil.Clear();
            foreach (var perfilId in adminUsuarioUpdateRequestDTO.PerfilIds)
            {
                entidade.UsuarioPerfil.Add(new UsuarioPerfil { PerfilId = perfilId });
            }
            await _usuarioRepository.AtualizaEntidadeUsuarioAsync(entidade);
            _logger.LogInformation("Admin: Usuário ID [{UsuarioId}] atualizado com sucesso.", id);

            return await _usuarioRepository.ObterUsuarioDtoPorIdAsync(id);
        }

        public async Task<UsuarioDTO?> AtualizarUsuarioAsync(int idUsuarioLogado, int idReq, UsuarioUpdateRequestDTO usuarioUpdateRequestDTO)
        {
            if (idReq != idUsuarioLogado) {
                var msg = $"Usuário: Falha na atualização para o usuário ID [{idReq}]: O usuário [{idUsuarioLogado}] tentou atualizar um usuário diferente";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }

            var entidade = await _usuarioRepository.ObterUsuarioEntidadePorIdAsync(idUsuarioLogado);

            if (entidade is null)
            {
                _logger.LogWarning("Tentativa de atualização para um usuário logado que não foi encontrado no banco. ID: [{UsuarioId}]", idUsuarioLogado);
                return null;
            }

            if (await _usuarioRepository.EmailExistsInAnotherUserAsync(idUsuarioLogado, usuarioUpdateRequestDTO.Email))
            {
                var msg = $"Falha na atualização para o usuário ID [{idUsuarioLogado}]: o e-mail [{usuarioUpdateRequestDTO.Email}] já está em uso"; 
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }

            entidade.NomeCompleto = usuarioUpdateRequestDTO.NomeCompleto;
            entidade.Email = usuarioUpdateRequestDTO.Email;
            entidade.DataAtualizacao = DateTime.UtcNow;
            _logger.LogInformation("Usuário ID [{UsuarioId}] iniciando processo de atualização do próprio cadastro.", idUsuarioLogado);
            await _usuarioRepository.AtualizaEntidadeUsuarioAsync(entidade);
            _logger.LogInformation("Perfil do usuário ID [{UsuarioId}] atualizado com sucesso.", idUsuarioLogado);

            return await _usuarioRepository.ObterUsuarioDtoPorIdAsync(idUsuarioLogado);
        }
    }
}
