using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public DbSet<UserEntity> UserEntity { get; set; }
        public DbSet<RoleEntity> RoleEntity { get; set; }
        public DbSet<UserRoleEntity> UserRoleEntity { get; set; }
        public DbSet<ClientSystemEntity> ClientSystemEntity { get; set; }
        public DbSet<FeatureEntity> FeatureEntity { get; set; }
        public DbSet<AuthenticationProviderEntity> AuthenticationProvider { get; set; }
        public DbSet<TenantEntity> TenantEntity { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //UserEntity
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("tbl_users");
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            //RoleEntity
            modelBuilder.Entity<RoleEntity>(entity =>
            {
                entity.ToTable("tbl_roles");
            });

            //UserRoleEntity
            modelBuilder.Entity<UserRoleEntity>(entity =>
            {
                entity.ToTable("tbl_user_x_role");
                entity.HasKey(up => new { up.UserId, up.RoleId });
            });

            //ClientSystemEntity
            modelBuilder.Entity<ClientSystemEntity>(entity =>
            {
                entity.ToTable("tbl_client_systems");
                entity.HasIndex(cs => cs.Name).IsUnique();
                entity.HasIndex(cs => cs.ClientId).IsUnique();
            });

            //FeatureEntity
            modelBuilder.Entity<FeatureEntity>(entity =>
            {
                entity.ToTable("tbl_features");
                entity.HasIndex(f => f.Name).IsUnique();
            });
            //RoleFeatureEntity
            modelBuilder.Entity<RoleFeatureEntity>(entity =>
            {
                entity.ToTable("tbl_role_x_feature");
                entity.HasKey(rf => new { rf.RoleId, rf.FeatureId, rf.ClientSystemId });
            });
            //PasswordPolicyEntity
            modelBuilder.Entity<PasswordPolicyEntity>(entity =>
            {
                entity.ToTable("tbl_password_policies");
            });
            //AuthenticationProviderEntity
            modelBuilder.Entity<AuthenticationProviderEntity>(entity =>
            {
                entity.ToTable("tbl_authentication_providers");
                entity.HasIndex(ap => ap.Name).IsUnique();
            });
            //UserProviderEntity
            modelBuilder.Entity<UserProviderEntity>(entity =>
            {
                entity.ToTable("tbl_user_x_provider");
                entity.HasKey(up => new { up.UserId, up.AuthenticationProviderId });
            });
            //TenantEntity
            modelBuilder.Entity<TenantEntity>(entity =>
            {
                entity.ToTable("tbl_tenants");
                entity.HasIndex(f => f.Name).IsUnique();
            });
        }
    }
}