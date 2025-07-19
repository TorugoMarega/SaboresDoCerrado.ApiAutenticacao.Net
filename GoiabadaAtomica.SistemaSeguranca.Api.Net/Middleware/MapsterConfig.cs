using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using Mapster;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // --- REGRA DE MAPEAMENTO CUSTOMIZADA ---
            //converter a coleção de entidades para uma lista de strings.
            config.NewConfig<User, UserDTO>()
                .Map(
                    dest => dest.Roles, // Para o destino "Perfis" (a lista de strings no DTO)
                    src => src.UserRole.Select(up => up.Role.Name).ToList() // Use a origem "UsuarioPerfil" e extraia apenas o nome de cada perfil
                );
        }
    }
}
