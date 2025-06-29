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

        public async Task RegistrarUsuarioAsync(Usuario usuario)
        {
            _contextoAplicacao.Add(usuario);
            await _contextoAplicacao.SaveChangesAsync();
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

        public async Task<UsuarioDTO> ObterPorIdAsync(int id)
        {
            return await _contextoAplicacao.Usuario
                .Where(usuario => usuario.Id == id)
                .Select(usuario => new UsuarioDTO
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Perfis = usuario.UsuarioPerfil.Select(up => up.Perfil.Nome).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}
