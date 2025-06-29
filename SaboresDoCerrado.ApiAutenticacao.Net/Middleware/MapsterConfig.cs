using Mapster;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // --- REGRA DE MAPEAMENTO CUSTOMIZADA ---
            //converter a coleção de entidades para uma lista de strings.
            config.NewConfig<Usuario, UsuarioDTO>()
                .Map(
                    dest => dest.Perfis, // Para o destino "Perfis" (a lista de strings no DTO)
                    src => src.UsuarioPerfil.Select(up => up.Perfil.Nome).ToList() // Use a origem "UsuarioPerfil" e extraia apenas o nome de cada perfil
                );
        }
    }
}
