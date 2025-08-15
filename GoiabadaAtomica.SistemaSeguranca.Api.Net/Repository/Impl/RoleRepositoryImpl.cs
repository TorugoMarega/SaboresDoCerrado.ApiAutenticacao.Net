using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl
{
    public class RoleRepositoryImpl : IRoleRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<RoleRepositoryImpl> _logger;

        public RoleRepositoryImpl(ApplicationContext context, ILogger<RoleRepositoryImpl> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync(int clientSystemId)
        {
            _logger.LogInformation("Iniciando busca de todos os perfis no banco de dados");
            var roles = await _context.RoleEntity
                .AsNoTracking()
                .Where(role => role.ClientSystemId == clientSystemId)
                .Select(role => new RoleDTO
                {
                    Id = role.Id,
                    Descritption = role.Description,
                    Name = role.Name,
                    IsActive = role.IsActive,
                    ClientSystemId = clientSystemId
                })
                .ToListAsync();
            return roles;
        }

        public async Task<RoleDTO?> GetRoleByIdAsync(int clientSystemId, int roleId)
        {
            _logger.LogInformation("Iniciando busca do perfil por ID [{roleId}] no banco de dados", roleId);
            var role = await _context.RoleEntity
                .AsNoTracking()
                .Where(role => role.Id == roleId && role.ClientSystemId == clientSystemId)
                .Select(role => new RoleDTO
                {
                    Id = role.Id,
                    Descritption = role.Description,
                    Name = role.Name,
                    IsActive = role.IsActive,
                    ClientSystemId = roleId
                })
                .FirstOrDefaultAsync();
            return role;
        }
        public async Task<RoleEntity?> GetRoleEntityByIdAsync(int clientSystemId, int roleId)
        {
            _logger.LogInformation("Iniciando busca do perfil por ID [{roleId}] no banco de dados", roleId);
            var role = await _context.RoleEntity
                .Where(role => role.Id == roleId && role.ClientSystemId == clientSystemId)
                .FirstOrDefaultAsync();
            return role;
        }

        public async Task<int> CountRolesAsync(int clientSystemId, List<int> roleIds)
        {
            if (roleIds == null || roleIds.Count == 0)
            {
                return 0;
            }

            return await _context.RoleEntity
                .AsNoTracking()
                .Where(role => role.ClientSystemId == clientSystemId)
                .CountAsync(role => roleIds.Contains(role.Id));
        }

        public async Task<bool> ExistsRoleByNameAsync(int clientSystemId, string Name, int? idToExclude = null)
        {
            IQueryable<RoleEntity> query = _context.RoleEntity.AsNoTracking();
            query = query.Where(role => role.ClientSystemId == clientSystemId && role.Name.ToLower() == Name.ToLower());

            if (idToExclude.HasValue)
            {
                query = query.Where(p => p.Id != idToExclude.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<RoleEntity> CreateRoleAsync(RoleEntity role)
        {
            await _context.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsRoleInUseByIdAsync(int roleId)
        {
            _logger.LogInformation("Iniciando busca do perfil em uso por ID [{roleId}] no banco de dados", roleId);
            var roleInUse = await _context.UserRoleEntity
                .AsNoTracking()
                .Where(userRole => userRole.RoleId == roleId)
                .AnyAsync();
            return roleInUse;
        }
    }
}