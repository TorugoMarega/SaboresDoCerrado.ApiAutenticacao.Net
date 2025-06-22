using Microsoft.EntityFrameworkCore;
using SaboresDoCerrado.ApiAutenticacao.Net.Data;
using SaboresDoCerrado.ApiAutenticacao.Net.Model;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public class PerfilRepository:IPerfilRepository
    {
        private readonly ContextoAplicacao _contexto;

        public PerfilRepository(ContextoAplicacao contexto)
        {
            _contexto = contexto;
        }
        public async Task<IEnumerable<Perfil>> ObterTodosAsync()
        {
            return await _contexto.Perfis.ToListAsync();
        }
    }
}
