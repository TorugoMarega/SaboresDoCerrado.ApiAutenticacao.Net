using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>().HasKey(up => new { up.UserId, up.RoleId });

            modelBuilder.Entity<User>().ToTable("tbl_users");
            modelBuilder.Entity<Role>().ToTable("tbl_roles");
            modelBuilder.Entity<UserRole>().ToTable("tbl_user_x_role");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique(); // UNIQUE

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email) // UNIQUE
                .IsUnique();
        }
    }
}