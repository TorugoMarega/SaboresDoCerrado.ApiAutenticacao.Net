namespace SaboresDoCerrado.ApiAutenticacao.Net.Model.entity
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string NomeUsuario { get; set; }
        public required string NomeCompleto { get; set; }
        public required string HashSenha { get; set; }
        public required string Email { get; set; }
        public required DateTime DataCriacao { get; set; }
        public required DateTime DataAtualizacao { get; set; }
        public ICollection<UsuarioPerfil> UsuarioPerfil { get; set; }
    }
}
