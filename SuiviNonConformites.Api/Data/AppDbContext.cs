using Microsoft.EntityFrameworkCore;
using SuiviNonConformites.Api.Models;

namespace SuiviNonConformites.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Audit> Audits => Set<Audit>();
    public DbSet<NonConformite> NonConformites => Set<NonConformite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NonConformite>() //"je configure des règles concernant l'entité NonConformite".
            .HasOne(nc => nc.Audit) //"Chaque NonConformite a une relation vers un Audit" — via sa navigation property Audit. nc => nc.Audit est une expression lambda qui pointe vers la propriété concernée (pas besoin d'écrire son nom en chaîne de caractères, ce qui rendrait le refactoring dangereux).
            .WithMany(a => a.NonConformites) //"...et cet Audit peut avoir plusieurs NonConformite en retour" - via la navigation property NonConformites côté Audit. C'est cette ligne qui établit concrètement le sens one-to-many.
            .HasForeignKey(nc => nc.AuditId); //"...et la colonne qui matérialise cette relation en base est AuditId."
    }
}