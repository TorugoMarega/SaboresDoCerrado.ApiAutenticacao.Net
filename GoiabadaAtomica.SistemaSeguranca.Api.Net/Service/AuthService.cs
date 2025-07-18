using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.response;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.ApiAutenticacao.Net.Repository;
using MapsterMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration, ILogger<AuthService> logger, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<UserDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            _logger.LogInformation("Iniciando processo de registro para o usuário: [{Username}]", registrationRequestDTO.Username);

            if (await _userRepository.UsernameExistsAsync(registrationRequestDTO.Username))
            {
                throw new InvalidOperationException("O nome de usuário já está em uso");
            }
            if (await _userRepository.EmailExistsAsync(registrationRequestDTO.Email))
            {
                throw new InvalidOperationException("O e-mail já está cadastrado na base");
            }

            var existentRolesCount = await _roleRepository.CountRolesAsync(registrationRequestDTO.RoleIds);
            if (existentRolesCount != registrationRequestDTO.RoleIds.Count)
            {
                _logger.LogWarning("Tentativa de registro com um ou mais IDs de perfil inválidos.");
                throw new InvalidOperationException("Um ou mais perfis fornecidos são inválidos.");
            }

            _logger.LogInformation("Validações de unicidade aprovadas para [{Email}/{Username}]", registrationRequestDTO.Email, registrationRequestDTO.Username);
            _logger.LogDebug("Usuario novo");
            _logger.LogDebug("Tentando converter DTO para Entidade");
            var userEntity = new User
            {
                Username = registrationRequestDTO.Username,
                FullName = registrationRequestDTO.FullName,
                Email = registrationRequestDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationRequestDTO.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserRole = registrationRequestDTO.RoleIds.Select(role => new UserRole { RoleId = role }).ToList()
            };
            _logger.LogDebug("Tentando persistir usuario");
            var registeredUser = await _userRepository.RegisterUserAsync(userEntity);
            _logger.LogDebug("Usuario persistido com sucesso, ID: [{UserId}]", registeredUser.Id);
            var userDTO = await _userRepository.GetUserByIdAsync(registeredUser.Id);
            if (userDTO is null)
            {
                _logger.LogError("Falha crítica: Não foi possível encontrar o usuário com ID [{UserId}] imediatamente após o registro.", userEntity.Id);
                throw new ApplicationException("Erro ao recuperar usuário após o registro.");
            }
            return userDTO;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            _logger.LogInformation("Iniciando tentativa de login para o usuário [{username}]", loginRequestDTO.Username);

            var user = await _userRepository.GetUserByUsernameAsync(loginRequestDTO.Username);

            if (user is null)
            {
                var loginMsg = $"Usuário [{loginRequestDTO.Username}] não existe na base!";
                _logger.LogWarning("Tentativa de login falhou: {msg}", loginMsg);
                throw new InvalidOperationException(loginMsg);
            }
            if (user.IsActive is false)
            {
                var loginMsg = $"Usuário [{loginRequestDTO.Username}] está inativo, por favor entre em contato com um Administrador!";
                _logger.LogWarning("Tentativa de login falhou: {msg}", loginMsg);
                throw new InvalidOperationException(loginMsg);
            }
            _logger.LogDebug("Comparando senha");
            if (!BCrypt.Net.BCrypt.Verify(loginRequestDTO.Password, user.PasswordHash))
            {
                return null;
            }
            return new LoginResponseDTO
            {
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<bool> UpdateUserPasswordByIdAsync(int Id, UpdateUserPasswordRequestDTO updateUserPasswordRequestDTO)
        {
            _logger.LogInformation("Buscando usuário [{UserId}]", Id);
            var userEntity = await _userRepository.GetUserEntityByIdAsync(Id);
            if (userEntity is null)
            {
                _logger.LogWarning("Tentando atualizar senha de um usuário não cadastrado na base. Usuario [{UserId}]", Id);
                return false;
            }
            _logger.LogInformation("Validando senha atual do usuário [{UserId}]", Id);
            if (!BCrypt.Net.BCrypt.Verify(updateUserPasswordRequestDTO.CurrentPassword, userEntity.PasswordHash))
            {
                var msg = $"Falha na alteração de senha para o usuário ID [{Id}]: Senha atual incorreta.";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }

            _logger.LogDebug("Senha atual verificada com sucesso. Gerando novo hash para o usuário ID [{UserId}]", Id);
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserPasswordRequestDTO.NewPassword);
            userEntity.PasswordHash = newPasswordHash;
            userEntity.UpdatedAt = DateTime.UtcNow;
            _logger.LogInformation("Tentando atualizar senha usuario pelo id [{UserId}]", Id);
            await _userRepository.UpdateUserAsync(userEntity);

            _logger.LogInformation("Senha do usuário ID [{UserId}] alterada com sucesso.", Id);
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (user.UserRole is not null && user.UserRole.Any())
            {
                foreach (var userRole in user.UserRole)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                }
            }

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = credentials
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            _logger.LogDebug("Gerando Token");
            var tokenString = tokenHandler.WriteToken(securityToken);

            return tokenString;
        }
    }
}