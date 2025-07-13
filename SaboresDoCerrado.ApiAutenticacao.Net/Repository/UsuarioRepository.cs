using Mapster;
using Microsoft.EntityFrameworkCore;
using SaboresDoCerrado.ApiAutenticacao.Net.Data;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request;
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

        public async Task<UsuarioDTO?> ObterPorIdNoTrackAsync(int id)
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
        public async Task<Usuario?> ObterPorIdAsync(int id)
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

        public async Task<UsuarioDTO?> ObterUsuarioPorNomeUsuarioAsync(string NomeUsuario)
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

        public async Task<LoginDTO?> ObterUsuarioLoginAsync(string NomeUsuario)
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
        public async Task<UsuarioDTO?> UpdateUsuarioPorId(int id, UsuarioUpdateRequestDTO usuarioUpdateRequestDTO)
        {
            var entidade = await ObterPorIdAsync(id);
            if (entidade is null)
            {
                return null;
            }

            if (await EmailExistsInAnotherUserAsync(id, usuarioUpdateRequestDTO.Email))
            {
                var msg = "Este email já está cadastrado para outro usuário";
                throw new InvalidOperationException(msg);
            }

            usuarioUpdateRequestDTO.Adapt(entidade);
            entidade.DataAtualizacao = DateTime.UtcNow;

            entidade.UsuarioPerfil.Clear();
            var novosPerfis = usuarioUpdateRequestDTO.PerfilIds.Select(perfilId => new UsuarioPerfil { PerfilId = perfilId }).ToList();
            foreach (var perfil in novosPerfis)
            {
                entidade.UsuarioPerfil.Add(perfil);
            }

            await _contextoAplicacao.SaveChangesAsync();
            return await ObterPorIdNoTrackAsync(id);
        }
    }
}
