using Mapster;
using Microsoft.EntityFrameworkCore;
using SaboresDoCerrado.ApiAutenticacao.Net.Data;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
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

        public async Task<IEnumerable<UsuarioDTO>> ObterTodosUsuariosDtoAsync()
        {
            return await _contextoAplicacao.Usuario
                   .AsNoTracking()
                   .Select(usuario => new UsuarioDTO
                   {
                       Id = usuario.Id,
                       NomeUsuario = usuario.NomeUsuario,
                       NomeCompleto = usuario.NomeCompleto,
                       IsAtivo = usuario.IsAtivo,
                       Email = usuario.Email,
                       Perfis = usuario.UsuarioPerfil.Select(up => up.Perfil.Nome).ToList()
                   })
                   .ToListAsync();
        }

        public async Task<UsuarioDTO?> ObterUsuarioDtoPorIdAsync(int id)
        {
            return await _contextoAplicacao.Usuario
                .AsNoTracking()
                .Where(usuario => usuario.Id == id)
                .Select(usuario => new UsuarioDTO
                {
                    Id = usuario.Id,
                    NomeUsuario = usuario.NomeUsuario,
                    NomeCompleto = usuario.NomeCompleto,
                    IsAtivo = usuario.IsAtivo,
                    Email = usuario.Email,
                    Perfis = usuario.UsuarioPerfil.Select(up => up.Perfil.Nome).ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<Usuario?> ObterUsuarioEntidadePorIdAsync(int id)
        {
            return await _contextoAplicacao.Usuario
                .Include(usuario => usuario.UsuarioPerfil)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _contextoAplicacao.Usuario.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> NomeUsuarioExistsAsync(string NomeUsuario)
        {
            return await _contextoAplicacao.Usuario.AnyAsync(usuario => usuario.NomeUsuario == NomeUsuario);
        }

        public async Task<UsuarioDTO?> ObterUsuarioDtoPorNomeAsync(string NomeUsuario)
        {
            return await _contextoAplicacao.Usuario
                .AsNoTracking()
                .Where(usuario => usuario.NomeUsuario.ToLower() == NomeUsuario.ToLower() & usuario.IsAtivo)
                .Select(usuario => new UsuarioDTO
                {
                    Id = usuario.Id,
                    NomeUsuario = usuario.NomeUsuario,
                    NomeCompleto = usuario.NomeCompleto,
                    IsAtivo = usuario.IsAtivo,
                    Email = usuario.Email,
                    Perfis = usuario.UsuarioPerfil.Select(up => up.Perfil.Nome).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<string?> VerificarConflitoAsync(string NomeUsuario, string Email)
        {
            var UsuarioExistente = await _contextoAplicacao.Usuario
                .AsNoTracking()
                .FirstOrDefaultAsync(usuario => usuario.NomeUsuario.ToLower() == NomeUsuario.ToLower() || usuario.Email.ToLower() == Email.ToLower());
            if (UsuarioExistente is null)
            {
                return null;
            }
            // Se encontrou um usuário, verifica qual campo deu conflito e retorna a mensagem apropriada.
            if (UsuarioExistente.NomeUsuario.Equals(NomeUsuario, StringComparison.InvariantCultureIgnoreCase))
            {
                return "O nome de usuário já está em uso";
            }

            if (UsuarioExistente.Email.Equals(Email, StringComparison.InvariantCultureIgnoreCase))
            {
                return "O e-mail já está cadastrado na base";
            }

            return "Erro desconhecido de validação";
        }

        public async Task<LoginDTO?> ObterUsuarioDtoLoginAsync(string NomeUsuario)
        {
            return await _contextoAplicacao.Usuario
                .AsNoTracking()
                .Where(usuario => usuario.NomeUsuario.ToLower() == NomeUsuario.ToLower())
                .Select(usuario => new LoginDTO
                {
                    Id = usuario.Id,
                    NomeUsuario = usuario.NomeUsuario,
                    Email = usuario.Email,
                    IsAtivo = usuario.IsAtivo,
                    HashSenha = usuario.HashSenha,
                    Perfis = usuario.UsuarioPerfil.Select(up => up.Perfil.Nome).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> InativarAivarUsuarioAsync(int id, bool status)
        {
            var linhasAfetadas = await _contextoAplicacao.Usuario
                .Where(usuario => usuario.Id == id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(usuario => usuario.IsAtivo, status)
                .SetProperty(usuario => usuario.DataAtualizacao, DateTime.UtcNow)
                );
            // Se for > 0, o usuário foi encontrado e atualizado.
            return linhasAfetadas > 0;
        }
        public async Task<bool> EmailExistsInAnotherUserAsync(int id, string Email)
        {
            return await _contextoAplicacao.Usuario
                .AsNoTracking()
                .AnyAsync(
                usuario => usuario.Email.ToLower() == Email.ToLower() && usuario.Id != id
                );
        }
        public async Task AtualizaEntidadeUsuarioAsync(Usuario usuario) {
            _contextoAplicacao.Usuario.Update(usuario);
            await _contextoAplicacao.SaveChangesAsync();
        }
    }
}
