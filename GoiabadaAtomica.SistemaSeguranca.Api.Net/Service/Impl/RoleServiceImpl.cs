using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;
using MapsterMapper;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl
{
    public class RoleServiceImpl : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IClientSystemService _clientSystemService;
        private readonly ILogger<RoleServiceImpl> _logger;
        private readonly IMapper _mapper;

        public RoleServiceImpl(IRoleRepository roleRepository, IClientSystemService clientSystemService, ILogger<RoleServiceImpl> logger, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _clientSystemService = clientSystemService;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync(int tenantId, int clientSystemId)
        {
            _logger.LogInformation("Iniciando processo de listagem de Roles do sistema [{ClientSystemID}] da empresa [{TenantId}]", clientSystemId, tenantId);
            _logger.LogInformation("Iniciando validação de existência do sistema [{ClientSystemID}] da empresa [{TenantId}]", clientSystemId, tenantId);
            if (!await _clientSystemService.ExistsClientSystemByIdAsync(tenantId, clientSystemId))
            {
                throw new InvalidOperationException($"Não há foi encontrado registro da empresa [{tenantId}]/sistema [{clientSystemId}]");
            }
            _logger.LogInformation("Processo de listagem de Roles do sistema[{ClientSystemID}] da empresa[{TenantId}] finalizado com sucesso", clientSystemId, tenantId);
            return await _roleRepository.GetAllRolesAsync(clientSystemId);
        }

        public async Task<RoleDTO?> GetRoleByIdAsync(int tenantId, int clientSystemId, int roleId)
        {
            _logger.LogInformation("Iniciando processo de busca da Role [{RoleId}] do sistema [{ClientSystemID}] da empresa [{TenantId}]", roleId, clientSystemId, tenantId);
            _logger.LogInformation("Iniciando validação de existência do sistema [{ClientSystemID}] da empresa [{TenantId}]", clientSystemId, tenantId);
            if (!await _clientSystemService.ExistsClientSystemByIdAsync(tenantId, clientSystemId))
            {
                throw new InvalidOperationException($"Não há foi encontrado registro da empresa [{tenantId}]/sistema [{clientSystemId}]");
            }

            var role = await _roleRepository.GetRoleByIdAsync(clientSystemId, roleId);
            if (role == null)
            {
                _logger.LogInformation($"Role [{roleId}] não encontrada");
                return null;
            }
            _logger.LogInformation("Processo de listagem de Roles do sistema[{ClientSystemID}] da empresa[{TenantId}] finalizado com sucesso", clientSystemId, tenantId);
            return role;
        }

        public async Task<RoleDTO?> UpdateRoleByIdAsync(int clientSystemId, int tenantId, int roleId, UpdateRoleRequestDTO updateRoleRequestDTO)
        {
            _logger.LogInformation("Tentando atualizar o perfil [{id}]", roleId);

            _logger.LogInformation("Iniciando validação de existência do sistema [{ClientSystemID}] da empresa [{TenantId}]", clientSystemId, tenantId);
            if (!await _clientSystemService.ExistsClientSystemByIdAsync(tenantId, clientSystemId))
            {
                throw new InvalidOperationException($"Não há foi encontrado registro da empresa [{tenantId}]/sistema [{clientSystemId}]");
            }

            var roleEntity = await _roleRepository.GetRoleEntityByIdAsync(clientSystemId, roleId);
            if (roleEntity is null)
            {
                return null;
            }

            if (await _roleRepository.ExistsRoleByNameAsync(clientSystemId, updateRoleRequestDTO.Name, roleId))
            {
                throw new InvalidOperationException($"Há um perfil em uso com o nome [{updateRoleRequestDTO.Name}]");
            }

            _mapper.Map(updateRoleRequestDTO, roleEntity);

            await _roleRepository.SaveChangesAsync();

            return _mapper.Map<RoleDTO>(roleEntity);
        }

        public async Task<RoleDTO> DeactivateActivateRolesByIdAsync(int tenantId, int clientSystemId, int roleId, bool newStatus)
        {
            {
                var statusText = newStatus ? "ativação" : "desativação";
                _logger.LogInformation("Iniciando processo de [{StatusText}] para o Role ID [{RoleId}]", statusText, roleId);

                await _clientSystemService.ExistsClientSystemByIdAsync(tenantId, clientSystemId);

                var roleEntity = await _roleRepository.GetRoleEntityByIdAsync(clientSystemId, roleId);

                if (roleEntity is null || roleEntity.ClientSystemId != clientSystemId)
                {
                    _logger.LogWarning("Role ID [{RoleId}] não encontrado ou não pertence ao ClientSystem ID [{ClientSystemId}]", roleId, clientSystemId);
                    throw new KeyNotFoundException("Perfil não encontrado ou não pertence a este sistema.");
                }

                if (roleEntity.IsActive == newStatus)
                {
                    var currentStatusText = newStatus ? "ativo" : "inativo";
                    throw new InvalidOperationException($"O perfil já se encontra [{currentStatusText}].");
                }

                if (newStatus == false && await CheckRoleIUseAync(roleId))
                {
                    var msg = $"O perfil [{roleId}] está em uso por um ou mais usuários e não pode ser desativado.";
                    _logger.LogWarning(msg);
                    throw new InvalidOperationException(msg);
                }

                _logger.LogInformation("Alterando status do Role ID [{RoleId}] para [{NewStatus}]", roleId, newStatus);
                roleEntity.IsActive = newStatus;

                await _roleRepository.SaveChangesAsync();

                return roleEntity.Adapt<RoleDTO>();
            }
        }

        public async Task<RoleDTO> CreateRoleAsync(int tenantId, int clientSystemId, PostRoleRequestDTO postRoleRequestDTO)
        {
            _logger.LogInformation("Iniciando validação de existência do sistema [{ClientSystemID}] da empresa [{TenantId}]", clientSystemId, tenantId);
            if (!await _clientSystemService.ExistsClientSystemByIdAsync(tenantId, clientSystemId))
            {
                throw new InvalidOperationException($"Não há foi encontrado registro da empresa [{tenantId}]/sistema [{clientSystemId}]");
            }

            if (await _roleRepository.ExistsRoleByNameAsync(clientSystemId, postRoleRequestDTO.Name))
            {
                throw new InvalidOperationException($"Há um perfil em uso com o nome [{postRoleRequestDTO.Name}]");
            }

            var role = _mapper.Map<RoleEntity>(postRoleRequestDTO);
            role.IsActive = true;

            var newRoleEntity = await _roleRepository.CreateRoleAsync(role);

            return _mapper.Map<RoleDTO>(newRoleEntity);
        }

        public async Task<bool> CheckRoleIUseAync(int roleId)
        {
            return await _roleRepository.ExistsRoleInUseByIdAsync(roleId);
        }
    }
}