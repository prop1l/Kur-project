using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Database;

namespace ApiGradeProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenssController : ControllerBase
{
    private readonly PostgresContext _context;

    public TokenssController(PostgresContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Tokensses);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var token = _context.Tokensses.Find(id);
        if (token == null) return NotFound($"Токен с ID {id} не найден.");
        return Ok(token);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var token = _context.Tokensses.Find(id);
        if (token == null) return NotFound($"Токен с ID {id} не найден.");
        _context.Tokensses.Remove(token);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("all")]
    public IActionResult DeleteAll()
    {
        var tokens = _context.Tokensses.ToList();
        if (!tokens.Any()) return NotFound("Токены не найдены.");
        _context.Tokensses.RemoveRange(tokens);
        _context.SaveChanges();
        return NoContent();
    }
}