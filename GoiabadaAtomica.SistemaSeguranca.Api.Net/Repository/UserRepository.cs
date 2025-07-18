using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _applicationContext;

        public UserRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            //Adicionando entidade Usuario ao contexto do EF
            _applicationContext.User.Add(user);
            await _applicationContext.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            return await _applicationContext.User
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
            return await _applicationContext.User
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
        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            return await _applicationContext.User
                .Include(user => user.UserRole)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _applicationContext.User.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _applicationContext.User.AnyAsync(user => user.Username == username);
        }

        public async Task<UserDTO?> GetUserByUsernameAsync(string username)
        {
            return await _applicationContext.User
                .AsNoTracking()
                .Where(user => user.Username.ToLower() == username.ToLower() & user.IsActive)
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

        public async Task<string?> CheckConflictAsync(string username, string email)
        {
            var existingUser = await _applicationContext.User
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower() || user.Email.ToLower() == email.ToLower());
            if (existingUser is null)
            {
                return null;
            }
            // Se encontrou um usuário, verifica qual campo deu conflito e retorna a mensagem apropriada.
            if (existingUser.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
            {
                return "O nome de usuário já está em uso";
            }

            if (existingUser.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
            {
                return "O e-mail já está cadastrado na base";
            }

            return "Erro desconhecido de validação";
        }

        public async Task<LoginDTO?> GetUserByLoginAsync(string username)
        {
            return await _applicationContext.User
                .AsNoTracking()
                .Where(user => user.Username.ToLower() == username.ToLower())
                .Select(user => new LoginDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    PasswordHash = user.PasswordHash,
                    Roles = user.UserRole.Select(userRole => userRole.Role.Name).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeactivateActivateUserAsync(int id, bool status)
        {
            var affectedRows = await _applicationContext.User
                .Where(user => user.Id == id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(user => user.IsActive, status)
                .SetProperty(user => user.UpdatedAt, DateTime.UtcNow)
                );
            // Se for > 0, o usuário foi encontrado e atualizado.
            return affectedRows > 0;
        }
        public async Task<bool> EmailExistsInAnotherUserAsync(int id, string email)
        {
            return await _applicationContext.User
                .AsNoTracking()
                .AnyAsync(
                user => user.Email.ToLower() == email.ToLower() && user.Id != id
                );
        }
        public async Task UpdateUserAsync(User user)
        {
            _applicationContext.User.Update(user);
            await _applicationContext.SaveChangesAsync();
        }
    }
}
