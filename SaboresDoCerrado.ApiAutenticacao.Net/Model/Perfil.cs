namespace SaboresDoCerrado.ApiAutenticacao.Net.Model
{
    public class Perfil
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public ICollection<UsuarioPerfil> UsuarioPerfil { get; set; }
    }
}
