namespace SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public List<string> Perfis { get; set; }
    }
}
