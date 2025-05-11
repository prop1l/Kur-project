using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Database;

namespace ApiGradeProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserInfosController : ControllerBase
{
    private readonly PostgresContext _context;

    public UserInfosController(PostgresContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.UserInfos.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var info = _context.UserInfos.Find(id);
        if (info == null) return NotFound($"Информация о пользователе с ID {id} не найдена.");
        return Ok(info);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var info = _context.UserInfos.Find(id);
        if (info == null) return NotFound($"Информация о пользователе с ID {id} не найдена.");
        _context.UserInfos.Remove(info);
        _context.SaveChanges();
        return NoContent();
    }
}