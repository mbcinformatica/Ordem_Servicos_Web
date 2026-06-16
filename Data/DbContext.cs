using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Models;
<<<<<<< HEAD

namespace Ordem_Servicos_Web.Data
{
    public class MeuDbContext(DbContextOptions<MeuDbContext> options) : DbContext(options)
    {
=======
using System.Linq;

namespace Ordem_Servicos_Web.Data
{
    public class MeuDbContext : DbContext
    {
        public MeuDbContext(DbContextOptions<MeuDbContext> options)
            : base(options) { }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

        // DbSets para cada tabela
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Fornecedor> Fornecedores { get; set; } = null!;
<<<<<<< HEAD
        public DbSet<CategoriaServico> CategoriaServicos { get; set; } = null!;
=======
        public DbSet<CategoriaServico> CategoriasServicos { get; set; } = null!;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        public DbSet<LancamentoServico> LancamentosServicos { get; set; } = null!;
        public DbSet<Marca> Marcas { get; set; } = null!;
        public DbSet<Modelo> Modelos { get; set; } = null!;
        public DbSet<Produto> Produtos { get; set; } = null!;
        public DbSet<Servico> Servicos { get; set; } = null!;
        public DbSet<Unidade> Unidades { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Log> Logs { get; set; } = null!;
<<<<<<< HEAD
        public DbSet<Permissao> Permissoes { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;
        public DbSet<ItensMenu> ItensMenus { get; set; } = null!;
=======
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
<<<<<<< HEAD
            
=======

>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
            // Cliente: CPF/CNPJ único
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.CpfCnpj)
                .IsUnique();

            // Fornecedor: CPF/CNPJ único
            modelBuilder.Entity<Fornecedor>()
                .HasIndex(f => f.CpfCnpj)
                .IsUnique();

<<<<<<< HEAD
            modelBuilder.Entity<Fornecedor>()
                .HasIndex(fo => fo.NomeRazaoSocial)
                .IsUnique();

            // Marca: descrição única para evitar duplicatas
            modelBuilder.Entity<Marca>()
                .HasIndex(ma => ma.Descricao)
                .IsUnique();

            // Modelo: descrição única dentro da marca para evitar duplicatas
            modelBuilder.Entity<Modelo>()
                .HasOne(mo => mo.Marca)
                .WithMany(ma => ma.Modelos)
                .HasForeignKey(mo => mo.IdMarca);


            // Serviço: descrição única dentro da categoria de serviço para evitar duplicatas
            modelBuilder.Entity<Servico>()
                .HasOne(se => se.CategoriaServico)
                .WithMany(cs => cs.Servicos)
                .HasForeignKey(se => se.IdCategoriaServico);

            // Unidade: descrição única para evitar duplicatas
            modelBuilder.Entity<Unidade>()
                .HasIndex(un => un.Descricao)
                .IsUnique();

            // CategoriaServico: descrição única para evitar duplicatas
            modelBuilder.Entity<CategoriaServico>()
                .HasIndex(cs => cs.Descricao)
                .IsUnique();

            // Logs: índice por nível e data para facilitar consultas
            modelBuilder.Entity<Log>()
                .HasIndex(l => new { l.Level, l.Timestamp });

            // Permissões: relacionamentos
            modelBuilder.Entity<Permissao>()
                .HasOne(p => p.Menu)
                .WithMany(m => m.Permissoes)
                .HasForeignKey(p => p.IdMenu);

            modelBuilder.Entity<Permissao>()
                .HasOne(p => p.ItensMenu)
                .WithMany(i => i.Permissoes)
                .HasForeignKey(p => p.IdItensMenu);

            modelBuilder.Entity<Permissao>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Permissoes)
                .HasForeignKey(p => p.IdUsuario);

            modelBuilder.Entity<Produto>()
                .HasOne(pr => pr.Fornecedor)
                .WithMany(f => f.Produtos)
                .HasForeignKey(pr => pr.IdFornecedor);

            modelBuilder.Entity<Produto>()
                .HasOne(pr => pr.Marca)
                .WithMany(ma => ma.Produtos)
                .HasForeignKey(pr => pr.IdMarca);

            modelBuilder.Entity<Produto>()
                .HasOne(pr => pr.Modelo)
                .WithMany(mo => mo.Produtos)
                .HasForeignKey(pr => pr.IdModelo);

            modelBuilder.Entity<Produto>()
                .HasOne(pr => pr.Unidade)
                .WithMany(un => un.Produtos)
                .HasForeignKey(pr => pr.IdUnidade);
=======
            // Logs: índice por nível e data para facilitar consultas
            modelBuilder.Entity<Log>()
                .HasIndex(l => new { l.Level, l.Timestamp });
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        }

        public override int SaveChanges()
        {
<<<<<<< HEAD
            try
            {
                // Antes de salvar, percorre todas as entidades rastreadas
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        foreach (var prop in entry.Properties)
                        {
                            if (prop.Metadata.ClrType == typeof(string) && prop.CurrentValue != null)
                            {
                                prop.CurrentValue = prop.CurrentValue.ToString()?.Trim();
                            }
                        }
                    }
                }

                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar alterações: {ex.Message}");
                throw;
            }
        }
    }
}
=======
            // Antes de salvar, percorre todas as entidades rastreadas
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    foreach (var prop in entry.Properties)
                    {
                        if (prop.Metadata.ClrType == typeof(string) && prop.CurrentValue != null)
                        {
                            prop.CurrentValue = prop.CurrentValue.ToString();
                        }
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
