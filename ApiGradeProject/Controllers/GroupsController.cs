using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Database;

namespace ApiGradeProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly PostgresContext _context;

    public GroupsController(PostgresContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Groups.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var group = _context.Groups.Find(id);
        if (group == null) return NotFound($"Группа с ID {id} не найдена.");
        return Ok(group);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Group newGroup)
    {
        if (newGroup == null) return BadRequest("Неверные данные группы.");
        _context.Groups.Add(newGroup);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = newGroup.GroupId }, newGroup);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var group = _context.Groups.Find(id);
        if (group == null) return NotFound($"Группа с ID {id} не найдена.");
        _context.Groups.Remove(group);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Group updatedGroup)
    {
        if (updatedGroup == null || id != updatedGroup.GroupId)
            return BadRequest();

        var existingGroup = _context.Groups.Find(id);
        if (existingGroup == null) return NotFound();

        _context.Entry(existingGroup).CurrentValues.SetValues(updatedGroup);
        _context.SaveChanges();

        return NoContent();
    }
}