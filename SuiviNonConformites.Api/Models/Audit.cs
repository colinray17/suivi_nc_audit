namespace SuiviNonConformites.Api.Models;

public class Audit
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Client { get; set; } = string.Empty;
    public DateTime DateAudit { get; set; }

    public List<NonConformite> NonConformites { get; set; } = new();
}