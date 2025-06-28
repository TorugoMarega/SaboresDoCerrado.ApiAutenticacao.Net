using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.request;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public class UsuarioRepository:IUsuarioRepository
    {
        private readonly ContextoAplicacao _contextoAplicacao;

        public UsuarioRepository(ContextoAplicacao contextoAplicacao)
        {
            _contextoAplicacao = contextoAplicacao;
        }

        public async Task RegistrarUsuarioAsync(Usuario usuario) {
            _contextoAplicacao.Add(usuario);
            await _contextoAplicacao.SaveChangesAsync();
        }

        public async Task<IEnumerable<Usuario>> ObterTodosAsync() {
            return await _contextoAplicacao.Usuario.ToListAsync();
        }
    }
}
