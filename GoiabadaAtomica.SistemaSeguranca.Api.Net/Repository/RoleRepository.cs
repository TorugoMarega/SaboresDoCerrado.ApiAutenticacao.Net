using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(ApplicationContext context, ILogger<RoleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            _logger.LogInformation("Iniciando busca de todos os perfis no banco de dados");
            var roles = await _context.Role
                .AsNoTracking()
                .Select(role => new RoleDTO
                {
                    Id = role.Id,
                    Descritption = role.Description,
                    Name = role.Name,
                    Status = role.Status
                })
                .ToListAsync();
            return roles;
        }

        public async Task<RoleDTO?> GetRoleByIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca do perfil por ID [{roleId}] no banco de dados", id);
            var role = await _context.Role
                .AsNoTracking()
                .Where(role => role.Id == id)
                .Select(role => new RoleDTO
                {
                    Id = role.Id,
                    Descritption = role.Description,
                    Name = role.Name,
                    Status = role.Status
                })
                .FirstOrDefaultAsync();
            return role;
        }
        public async Task<Role?> GetRoleEntityByIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca do perfil por ID [{roleId}] no banco de dados", id);
            var role = await _context.Role
                .Where(role => role.Id == id)
                .FirstOrDefaultAsync();
            return role;
        }

        public async Task<int> CountRolesAsync(List<int> roleIds)
        {
            if (roleIds == null || roleIds.Count == 0)
            {
                return 0;
            }

            // Conta quantos dos IDs fornecidos existem na tabela de Perfis.
            return await _context.Role
                .AsNoTracking()
                .CountAsync(role => roleIds.Contains(role.Id));

        }
        public async Task<bool> ExistsRoleByNameAsync(string Name, int? idToExclude = null)
        {
            IQueryable<Role> query = _context.Role.AsNoTracking();
            query = query.Where(role => role.Name.ToLower() == Name.ToLower());
            //caso seja passado um id, ele sera excluido da query 
            if (idToExclude.HasValue)
            {
                query = query.Where(p => p.Id != idToExclude.Value);
            }
            return await query.AnyAsync();
        }
        public async Task<Role?> UpdateRoleByIdAsync(int id, UpdateRolelRequestDTO roleDTO)
        {
            var roleEntity = await GetRoleEntityByIdAsync(id);
            if (roleEntity is null)
            {
                return null;
            }
            if (await ExistsRoleByNameAsync(roleDTO.Name, id))
            {
                throw new InvalidOperationException($"Há um perfil em uso com o nome [{roleDTO.Name}]");
            }
            roleDTO.Adapt(roleEntity);
            await _context.SaveChangesAsync();
            return roleEntity;
        }
        public async Task<bool?> DeactivateActivateRoleAsync(int id, bool newStatus)
        {
            var role = await _context.Role.FirstOrDefaultAsync(role => role.Id == id);
            if (role is not null)
            {
                if (role.Status.Equals(newStatus)) return false;

                role.Status = newStatus;
                await _context.SaveChangesAsync();
                return true;
            }
            else return null;
        }
        public async Task<Role?> CreateRolelAsync(Role role)
        {
            if (await ExistsRoleByNameAsync(role.Name))
            {
                throw new InvalidOperationException($"Há um perfil em uso com o nome [{role.Name}]");
            }
            await _context.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }
    }
}
