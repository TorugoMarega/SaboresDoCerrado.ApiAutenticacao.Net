using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly ApplicationContext _applicationContext;

        public UserRepositoryImpl(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<UserEntity> RegisterUserAsync(UserEntity user)
        {
            _applicationContext.UserEntity.Add(user);
            await _applicationContext.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync(int tenantId)
        {
            return await _applicationContext.UserEntity
                    .AsNoTracking()
                    .Where(user => user.TenantId == tenantId)
                    .Select(user => new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FullName = user.FullName,
                        IsActive = user.IsActive,
                        Email = user.Email,
                        TenantId = tenantId,
                        Roles = user.UserRole.Select(userRole => userRole.Role.Name).ToList()
                    })
                    .ToListAsync();
        }

        public async Task<UserDTO?> GetUserByIdAsync(int tenantId, int userId)
        {
            return await _applicationContext.UserEntity
                .AsNoTracking()
                .Where(user => user.Id == userId && user.TenantId == tenantId)
                .Select(user => new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    Email = user.Email,
                    TenantId = tenantId,
                    Roles = user.UserRole.Select(userRole => userRole.Role.Name).ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<UserEntity?> GetUserEntityByIdAsync(int tenantId, int userId)
        {
            return await _applicationContext.UserEntity
                .Include(user => user.UserRole)
                .FirstOrDefaultAsync(user => user.Id == userId && user.TenantId == tenantId);
        }

        public async Task<bool> EmailExistsAsync(int tenantId, string email)
        {
            return await _applicationContext.UserEntity.AnyAsync(u => u.Email == email && u.TenantId == tenantId);
        }

        public async Task<bool> UsernameExistsAsync(int tenantId, string username)
        {
            return await _applicationContext.UserEntity.AnyAsync(user => user.Username == username && user.TenantId == tenantId);
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(int tenantId, string username)
        {
            return await _applicationContext.UserEntity
                .Include(u => u.UserRole)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(user => user.TenantId == tenantId && user.Username.ToLower() == username.ToLower());
        }

        public async Task<bool> DeactivateActivateUserAsync(int tenantId, int userId, bool status)
        {
            var affectedRows = await _applicationContext.UserEntity
                .Where(user => user.Id == userId && user.TenantId == tenantId)
                .ExecuteUpdateAsync(update => update
                .SetProperty(user => user.IsActive, status)
                .SetProperty(user => user.UpdatedAt, DateTime.UtcNow)
                );
            return affectedRows > 0;
        }
        public async Task<bool> EmailExistsInAnotherUserAsync(int tenantId, int userId, string email)
        {
            return await _applicationContext.UserEntity
                .AsNoTracking()
                .AnyAsync(
                user => user.Email.ToLower() == email.ToLower() && user.Id != userId && user.TenantId == tenantId
                );
        }
        public async Task UpdateUserAsync(UserEntity user)
        {
            _applicationContext.UserEntity.Update(user);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<UserEntity?> GetByUsernameWithTenantAsync(int tenantId, string username)
        {
            return await _applicationContext.UserEntity
                .Include(u => u.Tenant) // carrega os dados da empresa
                .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Username == username);
        }
    }
}