namespace SaboresDoCerrado.ApiAutenticacao.Net.Model.entity
{
    public class Perfil
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Descricao { get; set; }
        public required bool Status { get; set; }
        public ICollection<UsuarioPerfil>? UsuarioPerfil { get; set; }
    }
}
