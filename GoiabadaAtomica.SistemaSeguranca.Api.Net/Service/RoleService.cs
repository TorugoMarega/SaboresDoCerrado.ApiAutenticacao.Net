using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.ApiAutenticacao.Net.Repository;
using Mapster;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;


        public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }


        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            _logger.LogInformation("Iniciando listagem de Perfis");
            var roles = await _roleRepository.GetAllRolesAsync();
            return roles.Adapt<IEnumerable<RoleDTO>>();
        }
        public async Task<RoleDTO> GetRoleByIdAsync(int id)
        {
            var roles = await _roleRepository.GetRoleByIdAsync(id);
            if (roles == null)
            {
                return null;
            }
            var roleDTO = roles.Adapt<RoleDTO>();
            return roleDTO;
        }
        public async Task<RoleDTO?> UpdateRoleByIdAsync(int id, UpdateRolelRequestDTO updateRolelRequestDTO)
        {
            _logger.LogInformation("Tentando atualizar o perfil [{id}]", id);
            var roleEntity = await _roleRepository.UpdateRoleByIdAsync(id, updateRolelRequestDTO);
            if (roleEntity is not null)
            {
                var roleDTO = roleEntity.Adapt<RoleDTO>();
                return roleDTO;
            }
            return null;
        }
        public async Task<bool?> DeactivateActivateRolesByIdAsync(int id, bool newStatus)
        {
            _logger.LogInformation("Tentando atualizar o status do perfil [{id}], para o status: [{status}]", id, newStatus);
            var success = await _roleRepository.DeactivateActivateRoleAsync(id, newStatus);
            if (success is null) _logger.LogWarning("Perfil [{id}] não encontrado!", id);
            if (success.Equals(false)) _logger.LogWarning("O perfil [{id}] já está no status: [{newStatus}]", id, newStatus);
            return success;
        }
        public async Task<RoleDTO> CreateRoleAsync(PostRoleRequestDTO postRoleRequestDTO)
        {
            var role = new Role
            {
                Name = postRoleRequestDTO.Name,
                Description = postRoleRequestDTO.Description,
                Status = true
            };
            var newRoleEntity = await _roleRepository.CreateRolelAsync(role);
            return newRoleEntity.Adapt(new RoleDTO());
        }
    }
}
