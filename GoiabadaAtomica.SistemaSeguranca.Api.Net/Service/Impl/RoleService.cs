using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;
using MapsterMapper;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            _logger.LogInformation("Iniciando listagem de Perfis");
            return await _roleRepository.GetAllRolesAsync();
        }

        public async Task<RoleDTO?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
            {
                return null;
            }
            return role;
        }

        public async Task<RoleDTO?> UpdateRoleByIdAsync(int id, UpdateRolelRequestDTO updateRolelRequestDTO)
        {
            _logger.LogInformation("Tentando atualizar o perfil [{id}]", id);

            var roleEntity = await _roleRepository.GetRoleEntityByIdAsync(id);
            if (roleEntity is null)
            {
                return null;
            }

            if (await _roleRepository.ExistsRoleByNameAsync(updateRolelRequestDTO.Name, id))
            {
                throw new InvalidOperationException($"Há um perfil em uso com o nome [{updateRolelRequestDTO.Name}]");
            }

            _mapper.Map(updateRolelRequestDTO, roleEntity);

            await _roleRepository.SaveChangesAsync();

            return _mapper.Map<RoleDTO>(roleEntity);
        }

        public async Task<bool?> DeactivateActivateRolesByIdAsync(int id, bool newStatus)
        {
            if (await CheckRoleIUseAync(id)) {
                var msg = $"O perfil [{id}] está em uso, operação inválida";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
                    }
            _logger.LogInformation("Tentando atualizar o status do perfil [{id}], para o status: [{status}]", id, newStatus);
            var role = await _roleRepository.GetRoleEntityByIdAsync(id);
            if (role is null)
            {
                return null;
            }

            if (role.Status.Equals(newStatus))
            {
                return false;
            }

            role.Status = newStatus;
            await _roleRepository.SaveChangesAsync();

            return true;
        }

        public async Task<RoleDTO> CreateRoleAsync(PostRoleRequestDTO postRoleRequestDTO)
        {
            if (await _roleRepository.ExistsRoleByNameAsync(postRoleRequestDTO.Name))
            {
                throw new InvalidOperationException($"Há um perfil em uso com o nome [{postRoleRequestDTO.Name}]");
            }

            var role = _mapper.Map<Role>(postRoleRequestDTO);
            role.Status = true;

            var newRoleEntity = await _roleRepository.CreateRolelAsync(role);

            return _mapper.Map<RoleDTO>(newRoleEntity);
        }

        public async Task<bool> CheckRoleIUseAync(int roleId)
        {
            return await _roleRepository.ExistsRoleInUseByIdAsync(roleId);
        }
    }
}