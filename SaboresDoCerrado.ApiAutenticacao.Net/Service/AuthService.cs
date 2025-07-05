using MapsterMapper;
using Microsoft.IdentityModel.Tokens;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.request;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.response;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            _logger.LogInformation("Iniciando tentativa de login para o usuário [{usuario}]", loginRequestDTO.NomeUsuario);
            var hashSenha = BCrypt.Net.BCrypt.HashPassword(loginRequestDTO.Senha);
            _logger.LogDebug("Buscando e validando usuario no banco de dados!");
            var loginDTO = await _userRepository.ObterUsuarioLoginAsync(loginRequestDTO.NomeUsuario);
            //usuario nao encontrado na base
            if (loginDTO is null)
            {
                var msgLogin = "Usuário [{usuario}] não existe na base!".Replace("{usuario}", loginRequestDTO.NomeUsuario);
                _logger.LogWarning("Tentativa de login falhou: {mensagem}", msgLogin);
                throw new InvalidOperationException(msgLogin);
            }
            _logger.LogDebug("Comparando senha");
            if (!BCrypt.Net.BCrypt.Verify(loginRequestDTO.Senha, loginDTO.HashSenha))
            {
                return null;
            }
            return new LoginResponseDTO
            {
                Token = GerarTokenJwt(loginDTO)
            };
        }

        private string GerarTokenJwt(LoginDTO loginDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, loginDto.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, loginDto.NomeUsuario),
                new Claim(JwtRegisteredClaimNames.Email, loginDto.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (loginDto.Perfis is not null && loginDto.Perfis.Any())
            {
                foreach (var usuarioPerfil in loginDto.Perfis)
                {
                    claims.Add(new Claim(ClaimTypes.Role, usuarioPerfil));
                }
            }

            var credenciais = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = credenciais
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            _logger.LogDebug("Gerando Token");
            var tokenString = tokenHandler.WriteToken(securityToken);

            return tokenString;
        }
    }
}
