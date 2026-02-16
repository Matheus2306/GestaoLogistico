using GestaoLogistico.Models;
using GestaoLogistico.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestaoLogistico.Data
{
    public class AplicationDbContext : IdentityDbContext<Usuario>
    {
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options) : base(options)
        {
        }

        // 📦 DbSets para entidades de Gestão Logística (adicione conforme necessário)
        // public DbSet<Produto> Produtos { get; set; }
        // public DbSet<Estoque> Estoques { get; set; }
        // public DbSet<Pedido> Pedidos { get; set; }
        // public DbSet<Fornecedor> Fornecedores { get; set; }
        // public DbSet<Transportadora> Transportadoras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔧 Configurações do Identity (personalizar tabelas)
            ConfigureIdentityTables(modelBuilder);

            // 🔍 Aplicar configurações de entidades (caso você use IEntityTypeConfiguration)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AplicationDbContext).Assembly);

            // 🗑️ Configurar soft delete global
            ConfigureSoftDelete(modelBuilder);
        }

        private void ConfigureIdentityTables(ModelBuilder modelBuilder)
        {
            // Configurar tabela de Usuários
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                
                // Índices únicos para melhor desempenho
                entity.HasIndex(u => u.CPF)
                    .IsUnique()
                    .HasDatabaseName("IX_Usuarios_CPF");
                
                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Usuarios_Email");
                
                // Configurações de propriedades
                entity.Property(u => u.NomeCompleto)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(u => u.CPF)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsFixedLength();

                // Valores padrão para auditoria
                entity.Property(u => u.CriadoEm)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Renomear outras tabelas do Identity para português
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("Roles");
            });
            
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UsuarioRoles");
            });
            
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UsuarioClaims");
            });
            
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UsuarioLogins");
            });
            
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UsuarioTokens");
            });
        }

        private void ConfigureSoftDelete(ModelBuilder modelBuilder)
        {
            // Aplicar query filter global para soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, nameof(ISoftDelete.Excluido)),
                        Expression.Constant(false));
                    
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(Expression.Lambda(body, parameter));
                }
            }
        }

        // ⏱️ Override SaveChanges para adicionar auditoria automática
        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                // Auditoria de criação e atualização
                if (entry.Entity is IAuditavel auditavel)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditavel.CriadoEm = now;
                        // TODO: Preencher CriadoPorId com o ID do usuário atual (IHttpContextAccessor)
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditavel.AtualizadoEm = now;
                        // TODO: Preencher AtualizadoPorId com o ID do usuário atual (IHttpContextAccessor)
                    }
                }

                // Soft delete
                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDelete)
                {
                    entry.State = EntityState.Modified;
                    softDelete.Excluido = true;
                    softDelete.ExcluidoEm = now;
                    // TODO: Preencher ExcluidoPorId com o ID do usuário atual (IHttpContextAccessor)
                }
            }
        }
    }
}
