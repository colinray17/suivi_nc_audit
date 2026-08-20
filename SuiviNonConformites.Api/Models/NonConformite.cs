namespace SuiviNonConformites.Api.Models;

public class NonConformite
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Gravite Gravite { get; set; }
    public StatutNonConformite Statut { get; set; } = StatutNonConformite.Ouverte;
    public DateTime DateEcheance { get; set; }

    public int AuditId { get; set; }
    public Audit? Audit { get; set; }
}