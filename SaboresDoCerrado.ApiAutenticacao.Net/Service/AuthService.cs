using Mapster;
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

        public AuthService(IUsuarioRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<UsuarioDTO> ResgistrarAsync(RegistroRequestDTO registroRequestDTO) {
            var usuarioEntidade = registroRequestDTO.Adapt<Usuario>();
            usuarioEntidade.DataCriacao = DateTime.Now;
            usuarioEntidade.DataAtualizacao = DateTime.Now;
            usuarioEntidade.HashSenha = registroRequestDTO.Senha;
            await _userRepository.RegistrarUsuarioAsync(usuarioEntidade);
            return usuarioEntidade.Adapt<UsuarioDTO>();
        }
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO) { 
            throw new System.NotImplementedException(); 
        }

    }
}
