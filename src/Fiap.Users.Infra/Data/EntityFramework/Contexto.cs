using Fiap.Users.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Users.Infra.Data.EntityFramework
{
  public class Contexto : DbContext
  {
    readonly string _defaultSchema;

    public static string DefaultSchema { get { return "dbo"; } }

    protected Contexto() : base() { }

    public Contexto(DbContextOptions<Contexto> options, string defaultSchema = "dbo") : base(options)
    {
      _defaultSchema = defaultSchema;

      this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<Usuario> Usuario { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.HasDefaultSchema(_defaultSchema);

      modelBuilder.Entity<Usuario>(entity =>
      {
        entity.HasKey(x => x.Id)
                  .HasName("PK_Usuario");

        entity.ToTable("Usuario", "dbo");

        entity.HasData(new Usuario(
            id: 1,
            nome: "Douglas ADM",
            email: "douglas.loc@hotmail.com",
            senha: "Admin@123",
            perfil: "Administrador",
            ativo: true,
            dataCadastro: new DateTime(2026, 10, 3, 10, 30, 0)
        ));
      });

      base.OnModelCreating(modelBuilder);
    }
  }
}