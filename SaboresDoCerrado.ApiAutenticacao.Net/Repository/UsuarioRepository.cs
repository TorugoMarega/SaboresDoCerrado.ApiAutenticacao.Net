using Microsoft.EntityFrameworkCore;
using SaboresDoCerrado.ApiAutenticacao.Net.Data;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ContextoAplicacao _contextoAplicacao;

        public UsuarioRepository(ContextoAplicacao contextoAplicacao)
        {
            _contextoAplicacao = contextoAplicacao;
        }

        public async Task<Usuario> RegistrarUsuarioAsync(Usuario usuario)
        {
            //Adicionando entidade Usuario ao contexto do EF
            _contextoAplicacao.Usuario.Add(usuario);
            await _contextoAplicacao.SaveChangesAsync();
            return usuario;
        }

        public async Task<IEnumerable<UsuarioDTO>> ObterTodosAsync()
        {
            return await _contextoAplicacao.Usuario
                   .Select(usuario => new UsuarioDTO
                   {
                       Id = usuario.Id,
                       Nome = usuario.Nome, // Garanta que os nomes das propriedades batem
                       Email = usuario.Email,
                       Perfis = usuario.UsuarioPerfil.Select(up => up.Perfil.Nome).ToList()
                   })
                   .ToListAsync();
        }

        public async Task<UsuarioDTO?> ObterPorIdAsync(int id)
        {
            return await _contextoAplicacao.Usuario
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Perfis = u.UsuarioPerfil.Select(up => up.Perfil.Nome).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _contextoAplicacao.Usuario.AnyAsync(u => u.Email == email);
        }
    }
}
