using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<UserServiceImpl> _logger;

        public UserServiceImpl(IUserRepository userRepository, IRoleRepository roleRepository, ILogger<UserServiceImpl> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            _logger.LogInformation("Listando todos os usuarios");
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Buscando usuario pelo id [{id}]", id);
            return await _userRepository.GetUserByIdAsync(id);
        }
        public async Task<bool> DeactivateActivateUserAsync(int id, bool newStatus)
        {
            var action = newStatus ? "ativação" : "inativação";
            _logger.LogInformation("Executando regra de negócio para [{action}] do usuário ID: [{UserId}]", action, id);
            return await _userRepository.DeactivateActivateUserAsync(id, newStatus);
        }
        public async Task<UserDTO?> UpdateUserAdminAsync(int id, UpdateUserAdminRequestDTO updateUserAdminRequestDTO)
        {
            _logger.LogInformation("Admin: Iniciando processo de atualização para o usuário ID: [{UserId}]", id);

            var userEntity = await _userRepository.GetUserEntityByIdAsync(id);
            if (userEntity is null)
            {
                _logger.LogWarning("Admin: Tentativa de atualizar usuário não existente. ID: [{UserId}]", id);
                return null;
            }

            if (await _userRepository.EmailExistsInAnotherUserAsync(id, updateUserAdminRequestDTO.Email))
            {
                _logger.LogWarning("Admin: Tentativa de atualizar usuário ID [{UserId}] com e-mail [{Email}] que já está em uso.", id, updateUserAdminRequestDTO.Email);
                throw new InvalidOperationException("O e-mail fornecido já está em uso por outro usuário.");
            }

            updateUserAdminRequestDTO.Adapt(userEntity);
            userEntity.UpdatedAt = DateTime.UtcNow;
            userEntity.UserRole.Clear();
            foreach (var roleId in updateUserAdminRequestDTO.RoleIds)
            {
                userEntity.UserRole.Add(new UserRoleEntity { RoleId = roleId });
            }
            await _userRepository.UpdateUserAsync(userEntity);
            _logger.LogInformation("Admin: Usuário ID [{UserId}] atualizado com sucesso.", id);

            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<UserDTO?> UpdateUserAsync(int loggedInUser, int idReq, UpdateUserRequestDTO updateUserRequestDTO)
        {
            if (idReq != loggedInUser)
            {
                var msg = $"Usuário: Falha na atualização para o usuário ID [{idReq}]: O usuário [{loggedInUser}] tentou atualizar um usuário diferente";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }

            var userEntity = await _userRepository.GetUserEntityByIdAsync(loggedInUser);

            if (userEntity is null)
            {
                _logger.LogWarning("Tentativa de atualização para um usuário logado que não foi encontrado no banco. ID: [{UserId}]", loggedInUser);
                return null;
            }

            if (await _userRepository.EmailExistsInAnotherUserAsync(loggedInUser, updateUserRequestDTO.Email))
            {
                var msg = $"Falha na atualização para o usuário ID [{loggedInUser}]: o e-mail [{updateUserRequestDTO.Email}] já está em uso";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }

            userEntity.FullName = updateUserRequestDTO.FullName;
            userEntity.Email = updateUserRequestDTO.Email;
            userEntity.UpdatedAt = DateTime.UtcNow;
            _logger.LogInformation("Usuário ID [{UserId}] iniciando processo de atualização do próprio cadastro.", loggedInUser);
            await _userRepository.UpdateUserAsync(userEntity);
            _logger.LogInformation("Perfil do usuário ID [{UserId}] atualizado com sucesso.", loggedInUser);

            return await _userRepository.GetUserByIdAsync(loggedInUser);
        }
    }
}