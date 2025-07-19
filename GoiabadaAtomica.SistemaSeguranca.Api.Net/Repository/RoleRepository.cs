using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
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

            return await _context.Role
                .AsNoTracking()
                .CountAsync(role => roleIds.Contains(role.Id));
        }

        public async Task<bool> ExistsRoleByNameAsync(string Name, int? idToExclude = null)
        {
            IQueryable<Role> query = _context.Role.AsNoTracking();
            query = query.Where(role => role.Name.ToLower() == Name.ToLower());

            if (idToExclude.HasValue)
            {
                query = query.Where(p => p.Id != idToExclude.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<Role> CreateRolelAsync(Role role)
        {
            await _context.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsRoleInUseByIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca do perfil em uso por ID [{roleId}] no banco de dados", id);
            var roleInUse = await _context.UserRole
                .AsNoTracking()
                .Where(userRole => userRole.RoleId == id)
                .AnyAsync();
            return roleInUse;
        }
    }
}