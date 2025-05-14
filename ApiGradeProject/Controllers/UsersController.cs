using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Database;
using ApiGradeProject.Script;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

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
        var users = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToList();

        return Ok(users);
    }


    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound($"Пользователь с ID {id} не найден.");
        return Ok(user);
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

    [HttpPost("create")]
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

        var now = DateTime.UtcNow;
        var expirationTime = now.AddMinutes(3);

        var emailToken = new Tokenss
        {
            TokenValue = token,
            UsersId = newUser.UserId,
            CreatedAt = now,
            ExpiresAt = expirationTime
        };

        _context.Tokensses.Add(emailToken);
        _context.SaveChanges();

        var body = $@"
                        <h3>Подтвердите ваш email</h3>
                        <p>Ваш код подтверждения:</p>
                        <h2>{token}</h2>
                        <p>Введите его в приложении для завершения регистрации.</p>
                        <br/>
                        <p>С уважением,<br/>Илья</p>";

        try
        {
            using var client = new SmtpClient("smtp.mail.ru", 587)
            {
                Credentials = new NetworkCredential("shuvaev-ilya@list.ru", "31uxVELvVjPKBAgNebPV"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("shuvaev-ilya@list.ru", "Grade"),
                Subject = "Подтверждение Email",
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(newUser.Email);

            client.Send(mailMessage);
        }
        catch (SmtpException smtpEx)
        {
            return StatusCode(500, $"Ошибка SMTP: {smtpEx.StatusCode} - {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Не удалось отправить email: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetById), new { id = newUser.UserId }, newUser);
    }

    [HttpGet("verify-token")]
    public IActionResult VerifyToken(int userId, string verifyToken)
    {
        var token = _context.Tokensses
            .FirstOrDefault(t => t.UsersId == userId && t.TokenValue == verifyToken);

        if (token == null)
        {
            return NotFound(new { Error = "Токен не найден." });
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            _context.Tokensses.Remove(token);
            _context.SaveChanges();
            return BadRequest(new { Error = "Срок действия токена истёк." });
        }

        var user = _context.Users.Find(userId);
        if (user == null)
        {
            _context.Tokensses.Remove(token);
            _context.SaveChanges();
            return NotFound(new { Error = "Пользователь не найден." });
        }

        user.IsEmailConfirmed = true;
        _context.Tokensses.Remove(token);
        _context.SaveChanges();

        return Ok(new { Message = "Почта успешно подтверждена!" });
    }

    [HttpGet("resend-token")]
    public async Task<IActionResult> ResendToken(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound(new { Error = "Пользователь не найден." });

        var existingToken = await _context.Tokensses.FirstOrDefaultAsync(t => t.UsersId == userId);
        if (existingToken != null)
        {
            _context.Tokensses.Remove(existingToken);
            await _context.SaveChangesAsync();
        }

        Random random = new();
        string token = random.Next(0, 1000000).ToString("D6");
        var expirationTime = DateTime.UtcNow.AddMinutes(3);

        var newToken = new Tokenss
        {
            TokenValue = token,
            UsersId = user.UserId,
            ExpiresAt = expirationTime
        };

        _context.Tokensses.Add(newToken);
        await _context.SaveChangesAsync();

        var body = $@"
                        <h3>Подтвердите ваш email</h3>
                        <p>Ваш код подтверждения:</p>
                        <h2>{token}</h2>
                        <p>Введите его в приложении для завершения регистрации.</p>
                        <br/>
                        <p>С уважением,<br/>Команда Grade</p>";

        try
        {
            using var client = new SmtpClient("smtp.mail.ru", 587)
            {
                Credentials = new NetworkCredential("shuvaev-ilya@list.ru", "31uxVELvVjPKBAgNebPV"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("shuvaev-ilya@list.ru", "Grade"),
                Subject = "Подтверждение Email",
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(user.Email);
            client.Send(mailMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = $"Не удалось отправить email: {ex.Message}" });
        }

        return Ok(new { Message = "Токен отправлен повторно." });
    }
}