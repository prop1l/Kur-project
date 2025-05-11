using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Database;
using ApiGradeProject.Script;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiGradeProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly PostgresContext _context;

    public UsersController(PostgresContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Users.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound($"Пользователь с ID {id} не найден.");
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Create([FromBody] User newUser)
    {
        if (newUser == null || string.IsNullOrWhiteSpace(newUser.Password) || string.IsNullOrWhiteSpace(newUser.Email))
            return BadRequest("Неверные данные пользователя.");

        if (_context.Users.Any(u => u.Email == newUser.Email))
            return Conflict("Пользователь с таким email уже существует.");

        newUser.Password = PasswordHash.HashPassword(newUser.Password);
        newUser.IsEmailConfirmed = false;
        _context.Users.Add(newUser);
        _context.SaveChanges();

        Random random = new();
        int randomNumber = random.Next(0, 1000000);
        string token = randomNumber.ToString("D6");
        var expirationTime = DateTime.UtcNow.AddHours(24);

        var emailToken = new Tokenss
        {
            TokenValue = token,
            UsersId = newUser.UserId,
            ExpiresAt = expirationTime
        };

        _context.Tokensses.Add(emailToken);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = newUser.UserId }, newUser);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound($"Пользователь с ID {id} не найден.");
        _context.Users.Remove(user);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] User updatedUser)
    {
        if (updatedUser == null) return BadRequest("Неверные данные.");
        var user = _context.Users.Find(id);
        if (user == null) return NotFound($"Пользователь с ID {id} не найден.");

        user.Login = updatedUser.Login;
        user.Email = updatedUser.Email;
        user.Password = PasswordHash.HashPassword(updatedUser.Password);
        user.IsEmailConfirmed = updatedUser.IsEmailConfirmed;

        _context.SaveChanges();
        return Ok(user);
    }

    [HttpPatch("{id}")]
    public IActionResult PartialUpdate(int id, [FromBody] JsonPatchDocument<User> patchDoc)
    {
        if (patchDoc == null) return BadRequest("Неверный JSON-Patch документ.");

        var user = _context.Users.Find(id);
        if (user == null) return NotFound($"Пользователь с ID {id} не найден.");

        patchDoc.ApplyTo(user);
        if (patchDoc.Operations.Any(op => op.path.Contains("password", StringComparison.OrdinalIgnoreCase)))
        {
            user.Password = PasswordHash.HashPassword(user.Password);
        }

        _context.SaveChanges();
        return Ok(user);
    }

    [HttpGet("authenticate")]
    public IActionResult Authenticate(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return NotFound($"Пользователь с email {email} не найден.");
        if (!PasswordHash.VerifyPassword(password, user.Password)) return Unauthorized("Неверный пароль.");
        return Ok(user);
    }

    [HttpGet("verify-token")]
    public IActionResult VerifyToken(int userId, string verifyToken)
    {
        var token = _context.Tokensses.FirstOrDefault(t => t.UsersId == userId && t.TokenValue == verifyToken);
        if (token == null) return NotFound("Токен не найден.");
        if (token.ExpiresAt < DateTime.UtcNow) return BadRequest("Срок действия токена истек.");

        var user = _context.Users.Find(userId);
        user.IsEmailConfirmed = true;
        _context.Tokensses.Remove(token);
        _context.SaveChanges();
        return Ok(new { Message = "Почта подтверждена!" });
    }
}