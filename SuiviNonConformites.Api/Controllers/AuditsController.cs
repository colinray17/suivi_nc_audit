using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuiviNonConformites.Api.Data;
using SuiviNonConformites.Api.Models;

namespace SuiviNonConformites.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/audits
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Audit>>> GetAll()
    {
        return Ok(await _context.Audits
            .Include(a => a.NonConformites)
            .ToListAsync());
    }

    // GET: api/audits/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Audit>> GetById(int id)
    {
        var audit = await _context.Audits
            .Include(a => a.NonConformites)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (audit is null) return NotFound();
        return Ok(audit);
    }

    // POST: api/audits
    [HttpPost]
    public async Task<ActionResult<Audit>> Create(Audit audit)
    {
        _context.Audits.Add(audit);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = audit.Id }, audit);
    }

    // DELETE: api/audits/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var audit = await _context.Audits.FindAsync(id);
        if (audit is null) return NotFound();

        _context.Audits.Remove(audit);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}