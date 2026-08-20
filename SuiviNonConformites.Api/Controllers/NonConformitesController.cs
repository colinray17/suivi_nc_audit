using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuiviNonConformites.Api.Data;
using SuiviNonConformites.Api.Models;

namespace SuiviNonConformites.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NonConformitesController : ControllerBase
{
    private readonly AppDbContext _context;

    public NonConformitesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/nonconformites
    // GET: api/nonconformites?statut=Ouverte
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NonConformite>>> GetAll([FromQuery] StatutNonConformite? statut) //[FromQuery] dit explicitement à ASP.NET Core d'aller chercher ce paramètre dans la query string de l'URL (?statut=Ouverte), plutôt que dans le body ou la route. Le ? sur StatutNonConformite? rend le paramètre optionnel — si tu appelles GET /api/nonconformites sans rien préciser, statut vaut null et aucun filtre n'est appliqué.
    {
        var query = _context.NonConformites
            .Include(nc => nc.Audit) //Contrairement au contrôleur Audits qui faisait .Include(a => a.NonConformites) (charger les enfants depuis le parent), ici on fait l'inverse : .Include(nc => nc.Audit) charge le parent depuis l'enfant, pour pouvoir afficher par exemple la référence de l'audit directement dans la liste des non-conformités côté Angular, sans requête supplémentaire.
            .AsQueryable();

        if (statut.HasValue)
        {
            query = query.Where(nc => nc.Statut == statut.Value);
        }

        return Ok(await query.ToListAsync());
    } //Point important : .Where(...) ne s'exécute pas immédiatement — EF Core construit progressivement une requête SQL, et c'est seulement .ToListAsync() (tout à la fin) qui déclenche réellement l'exécution en base, avec toutes les conditions accumulées. Ça permet de construire une requête conditionnelle sans dupliquer de code pour chaque cas (avec/sans filtre).

    // GET: api/nonconformites/5
    [HttpGet("{id}")]
    public async Task<ActionResult<NonConformite>> GetById(int id)
    {
        var nc = await _context.NonConformites
            .Include(nc => nc.Audit)
            .FirstOrDefaultAsync(nc => nc.Id == id);

        if (nc is null) return NotFound();
        return Ok(nc);
    }

    // POST: api/nonconformites
    [HttpPost]
    public async Task<ActionResult<NonConformite>> Create(NonConformite nonConformite)
    {
        var auditExiste = await _context.Audits.AnyAsync(a => a.Id == nonConformite.AuditId);
        if (!auditExiste)
        {
            return BadRequest($"Aucun audit trouvé avec l'id {nonConformite.AuditId}");
        } //Sans cette vérification, si on envoie un AuditId qui n'existe pas en base, ce n'est qu'au niveau de la contrainte SQL (la FOREIGN KEY qu'on a vue dans la migration) que ça échouerait — avec un message d'erreur SQLite peu clair pour l'utilisateur final. Ici, on vérifie en amont, en C#, et on renvoie une erreur 400 Bad Request explicite et compréhensible. C'est une bonne pratique : valider la logique métier avant de laisser la base de données faire respecter ses contraintes brutes

        _context.NonConformites.Add(nonConformite);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = nonConformite.Id }, nonConformite);
    }

    // PUT: api/nonconformites/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, NonConformite nonConformite)
    {
        if (id != nonConformite.Id) return BadRequest();

        _context.Entry(nonConformite).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/nonconformites/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var nc = await _context.NonConformites.FindAsync(id);
        if (nc is null) return NotFound();

        _context.NonConformites.Remove(nc);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}