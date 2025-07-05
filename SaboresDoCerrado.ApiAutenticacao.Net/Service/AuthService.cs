using MapsterMapper;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.request;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.response;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;

        public AuthService(IUsuarioRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<UsuarioDTO> ResgistrarAsync(RegistroRequestDTO registroRequestDTO)
        {
            _logger.LogInformation("Iniciando processo de registro para o usuário: [{usuario}]", registroRequestDTO.NomeUsuario);
            //valida existencia do email/nome de usuario
            var msgConflito = await _userRepository.VerificarConflitoAsync(registroRequestDTO.NomeUsuario, registroRequestDTO.Email);
            if (msgConflito is not null)
            {
                _logger.LogWarning("Tentativa de registro falhou: {mensagem}", msgConflito);
                throw new InvalidOperationException(msgConflito);
            }
            //caso nao exista
            _logger.LogInformation("Validações de unicidade aprovadas para [{Email}/{NomeUsuario}]", registroRequestDTO.Email, registroRequestDTO.NomeUsuario);
            _logger.LogDebug("Usuario novo");
            _logger.LogDebug("Tentando converter DTO para Entidade");
            var usuarioEntidade = new Usuario
            {
                NomeUsuario = registroRequestDTO.NomeUsuario,
                NomeCompleto = registroRequestDTO.NomeCompleto,
                Email = registroRequestDTO.Email,
                HashSenha = BCrypt.Net.BCrypt.HashPassword(registroRequestDTO.Senha),
                DataCriacao = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow,
                UsuarioPerfil = registroRequestDTO.PerfilIds.Select(pId => new UsuarioPerfil { PerfilId = pId }).ToList()
            };
            _logger.LogDebug("Tentando persistir usuario");
            var usuarioSalvo = _userRepository.RegistrarUsuarioAsync(usuarioEntidade);
            _logger.LogDebug("Usuario persistido com sucesso, ID: [{id}]", usuarioSalvo.Result.Id);
            var usuarioDTO = await _userRepository.ObterPorIdAsync(usuarioSalvo.Result.Id);
            if (usuarioDTO is null)
            {
                // Se chegarmos aqui, algo muito errado aconteceu.
                _logger.LogError("Falha crítica: Não foi possível encontrar o usuário com ID {UsuarioId} imediatamente após o registro.", usuarioEntidade.Id);
                throw new ApplicationException("Erro ao recuperar usuário após o registro.");
            }
            return usuarioDTO;

        }
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            _logger.LogInformation("Iniciando tentativa de login para o usuário {usuario}", loginRequestDTO.NomeUsuario);
            throw new System.NotImplementedException();
        }

    }
}
