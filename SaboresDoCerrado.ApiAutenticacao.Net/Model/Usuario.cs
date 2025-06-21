namespace SaboresDoCerrado.ApiAutenticacao.Net.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string HashSenha { get; set; }
        public string Email { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public ICollection<UsuarioPerfil> UsuarioPerfil { get; set; }
    }
}
