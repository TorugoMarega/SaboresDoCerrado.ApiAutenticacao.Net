using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITenantService _tenantService;
        private readonly ILogger<UserServiceImpl> _logger;

        public UserServiceImpl(IUserRepository userRepository, IRoleRepository roleRepository, ILogger<UserServiceImpl> logger, ITenantService tenantService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
            _tenantService = tenantService;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync(int tenantId)
        {
            _logger.LogInformation("Iniciando listagem de todos os usuarios da empresa [{TenantId}]", tenantId);
            ValidateTenantById(tenantId);
            _logger.LogInformation("Listagem finalizada para a empresa [{TenantId}]", tenantId);
            return await _userRepository.GetAllUsersAsync(tenantId);
        }

        public async Task<UserDTO?> GetUserByIdAsync(int tenantId, int userId)
        {
            _logger.LogInformation("Buscando usuario pelo id [{userId}]", userId);
            _logger.LogInformation("Iniciando listagem de todos os usuarios da empresa [{TenantId}]", tenantId);
            ValidateTenantById(tenantId);
            return await _userRepository.GetUserByIdAsync(tenantId, userId);
        }
        public async Task<bool> DeactivateActivateUserAsync(int tenantId, int userId, bool newStatus)
        {
            _logger.LogInformation("Iniciando atualização de status du usuário [{UserId}] da empresa [{TenantId}]",userId, tenantId);
            ValidateTenantById(tenantId);
            var action = newStatus ? "ativação" : "inativação";
            _logger.LogInformation("Executando regra de negócio para [{action}] do usuário ID: [{UserId}]", action, userId);
            return await _userRepository.DeactivateActivateUserAsync(tenantId, userId, newStatus);
        }
        public async Task<UserDTO?> UpdateUserAdminAsync(int tenantId, int userId, UpdateUserAdminRequestDTO updateUserAdminRequestDTO)
        {
            _logger.LogInformation("Admin: Iniciando processo de atualização para o usuário ID: [{UserId}] da empresa [{TenantId}]", userId, tenantId);
            
            ValidateTenantById(tenantId);

            var userEntity = await _userRepository.GetUserEntityByIdAsync(tenantId, userId);
            if (userEntity is null)
            {
                _logger.LogWarning("Admin: Tentativa de atualizar usuário não existente. ID: [{UserId}] ou não pertencente a empresa [{TenantId}]", userId, tenantId);
                return null;
            }

            if (await _userRepository.EmailExistsInAnotherUserAsync(tenantId, userId, updateUserAdminRequestDTO.Email))
            {
                _logger.LogWarning("Admin: Tentativa de atualizar usuário ID [{UserId}] com e-mail [{Email}] que já está em uso.", userId, updateUserAdminRequestDTO.Email);
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
            _logger.LogInformation("Admin: Usuário ID [{UserId}] atualizado com sucesso.", userId);

            return await _userRepository.GetUserByIdAsync(tenantId, userId);
        }

        public async Task<UserDTO?> UpdateUserAsync(int tenantId, int loggedInUser, int idReq, UpdateUserRequestDTO updateUserRequestDTO)
        {
            _logger.LogInformation("Iniciando processo de atualização do usuário [{UserId}] da empresa [{TenantId}]", loggedInUser, tenantId);
            if (idReq != loggedInUser)
            {
                var msg = $"Usuário: Falha na atualização para o usuário ID [{idReq}]: O usuário [{loggedInUser}] tentou atualizar um usuário diferente";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }

            ValidateTenantById(tenantId);

            var userEntity = await _userRepository.GetUserEntityByIdAsync(tenantId, loggedInUser);

            if (userEntity is null)
            {
                _logger.LogWarning("Tentativa de atualização para um usuário logado que não foi encontrado para a empresa informada. UserId: [{UserId}], TenantId[{TenantId}]", loggedInUser, tenantId);
                return null;
            }

            if (await _userRepository.EmailExistsInAnotherUserAsync(tenantId, loggedInUser, updateUserRequestDTO.Email))
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

            return await _userRepository.GetUserByIdAsync(tenantId, loggedInUser);
        }

        private async void ValidateTenantById(int tenantId) {
            _logger.LogInformation("Validando existência da empresa [{TenantId}]", tenantId);
            if (!await _tenantService.ExistsTenantByIdAsync(tenantId))
            {
                var msg = $"Empresa [{tenantId}] não localizada na base";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }
        }
    }
}