using Mapster;
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

        public AuthService(IUsuarioRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<UsuarioDTO> ResgistrarAsync(RegistroRequestDTO registroRequestDTO)
        {
            var usuarioEntidade = registroRequestDTO.Adapt<Usuario>();
            usuarioEntidade.DataCriacao = DateTime.UtcNow;
            usuarioEntidade.DataAtualizacao = DateTime.UtcNow;
            usuarioEntidade.HashSenha = BCrypt.Net.BCrypt.HashPassword(registroRequestDTO.Senha);
            usuarioEntidade.UsuarioPerfil = registroRequestDTO.PerfilIds.Select(perfilId => new UsuarioPerfil
            {
                PerfilId = perfilId
            }).ToList();

            await _userRepository.RegistrarUsuarioAsync(usuarioEntidade);
            var usuarioDTO = usuarioEntidade.Adapt<UsuarioDTO>();
            return usuarioDTO;
        }
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            throw new System.NotImplementedException();
        }

    }
}
