namespace SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string NomeUsuario { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public bool IsAtivo { get; set; }
        public List<string> Perfis { get; set; }
    }
}
