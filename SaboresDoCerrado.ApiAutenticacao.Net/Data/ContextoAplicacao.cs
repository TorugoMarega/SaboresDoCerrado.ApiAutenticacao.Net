using Microsoft.EntityFrameworkCore;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Data
{
    public class ContextoAplicacao : DbContext
    {
        public ContextoAplicacao(DbContextOptions<ContextoAplicacao> options) : base(options) { }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<UsuarioPerfil> UsuarioPerfis { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UsuarioPerfil>().HasKey(up => new { up.UsuarioId, up.PerfilId });

            modelBuilder.Entity<Usuario>().ToTable("tbl_usuario");
            modelBuilder.Entity<Perfil>().ToTable("tbl_perfil");
            modelBuilder.Entity<UsuarioPerfil>().ToTable("tbl_usuario_x_perfil");

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NomeUsuario)
                .IsUnique(); // UNIQUE

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email) // UNIQUE
                .IsUnique();
        }
    }
}