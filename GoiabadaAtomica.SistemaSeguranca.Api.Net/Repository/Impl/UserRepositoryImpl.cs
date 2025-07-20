using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
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

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            return await _applicationContext.UserEntity
                    .AsNoTracking()
                    .Select(user => new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FullName = user.FullName,
                        IsActive = user.IsActive,
                        Email = user.Email,
                        Roles = user.UserRole.Select(userRole => userRole.Role.Name).ToList()
                    })
                    .ToListAsync();
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            return await _applicationContext.UserEntity
                .AsNoTracking()
                .Where(user => user.Id == id)
                .Select(user => new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    Email = user.Email,
                    Roles = user.UserRole.Select(userRole => userRole.Role.Name).ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<UserEntity?> GetUserEntityByIdAsync(int id)
        {
            return await _applicationContext.UserEntity
                .Include(user => user.UserRole)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _applicationContext.UserEntity.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _applicationContext.UserEntity.AnyAsync(user => user.Username == username);
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(string username)
        {
            return await _applicationContext.UserEntity
                .Include(u => u.UserRole)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower());
        }

        public async Task<bool> DeactivateActivateUserAsync(int id, bool status)
        {
            var affectedRows = await _applicationContext.UserEntity
                .Where(user => user.Id == id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(user => user.IsActive, status)
                .SetProperty(user => user.UpdatedAt, DateTime.UtcNow)
                );
            return affectedRows > 0;
        }
        public async Task<bool> EmailExistsInAnotherUserAsync(int id, string email)
        {
            return await _applicationContext.UserEntity
                .AsNoTracking()
                .AnyAsync(
                user => user.Email.ToLower() == email.ToLower() && user.Id != id
                );
        }
        public async Task UpdateUserAsync(UserEntity user)
        {
            _applicationContext.UserEntity.Update(user);
            await _applicationContext.SaveChangesAsync();
        }
    }
}