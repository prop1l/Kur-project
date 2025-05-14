using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Script;
using Microsoft.AspNetCore.JsonPatch;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;


namespace ApiGradeProject.Controllers
{
    [Route("Api")]
    public class GradeControllers : ControllerBase
    {
        private readonly PostgresContext _context;

        public GradeControllers(PostgresContext context)
        {
            _context = context;
        }


        //
        // Groups Controller Methods
        //


        [HttpGet]
        [Route("/Groups")]
        public IActionResult GetAllGroups()
        {
            var groups = _context.Groups.ToList();
            return Ok(groups);
        }

        [HttpGet]
        [Route("/Roles")]
        public IActionResult GetAllRoles()
        {
            var roles = _context.Roles.ToList();
            return Ok(roles);
        }

        [HttpGet]
        [Route("/Groups/{id}")]
        public IActionResult GetGroupById(int id)
        {
            var group = _context.Groups.Find(id);
            if (group == null)
            {
                return NotFound($"Группа с ID {id} не найдена.");
            }
            return Ok(group);
        }

        [HttpPost]
        [Route("/Groups")]
        public IActionResult CreateGroup([FromBody] Group newGroup)
        {
            if (newGroup == null)
            {
                return BadRequest("Неверные данные группы.");
            }
            _context.Groups.Add(newGroup);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetGroupById), new { id = newGroup.GroupId }, newGroup);
        }

        [HttpDelete]
        [Route("/Groups/{id}")]
        public IActionResult DeleteGroup(int id)
        {
            var group = _context.Groups.Find(id);
            if (group == null)
            {
                return NotFound($"Группа с ID {id} не найдена.");
            }
            _context.Groups.Remove(group);
            _context.SaveChanges();
            return NoContent();
        }


        //
        // Users Controller Methods
        //


        [HttpGet]
        [Route("/Users")]
        public IActionResult GetAll()
        {
            var users = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToList();

            return Ok(users);
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

            var defaultRole = _context.Roles.FirstOrDefault(r => r.RoleName == "User");
            if (defaultRole == null)
                return StatusCode(500, "Роль 'User' не найдена в системе.");

            var userRole = new UserRole
            {
                UserId = newUser.UserId,
                RoleId = defaultRole.RoleId
            };

            _context.UserRoles.Add(userRole);
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
        <p>Введите его в приложении для завершения регистрации.</p>";

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

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, newUser);
        }

        [HttpPost]
        [Route("/Users")]
        public IActionResult RegisterUser([FromBody] User newUser)
        {
            if (newUser == null || string.IsNullOrWhiteSpace(newUser.Password) || string.IsNullOrWhiteSpace(newUser.Email))
            {
                return BadRequest("Неверные данные пользователя.");
            }
            if (_context.Users.Any(u => u.Email == newUser.Email))
            {
                return Conflict("Пользователь с таким email уже существует.");
            }
            newUser.Password = PasswordHash.HashPassword(newUser.Password);
            newUser.IsEmailConfirmed = false;
            _context.Users.Add(newUser);
            _context.SaveChanges();

            Random random = new Random();
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

            return Ok(new { Message = "Регистрация успешна. Пожалуйста, подтвердите ваш email.", Token = token });
        }

        [HttpGet]
        [Route("/Users/{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound($"Пользователь с ID {id} не найден.");
            }
            return Ok(user);
        }

        [HttpDelete]
        [Route("/Users/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound($"Пользователь с ID {id} не найден.");
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut]
        [Route("/Users/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (updatedUser == null)
            {
                return BadRequest("Неверные данные пользователя.");
            }
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound($"Пользователь с ID {id} не найден.");
            }
            user.Login = updatedUser.Login;
            user.Email = updatedUser.Email;
            user.Password = PasswordHash.HashPassword(updatedUser.Password);
            user.IsEmailConfirmed = updatedUser.IsEmailConfirmed;
            _context.SaveChanges();
            return Ok(user);
        }

        [HttpPatch]
        [Route("/Users/{id}")]
        public IActionResult PartiallyUpdateUser(int id, [FromBody] JsonPatchDocument<User> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Неверные данные для частичного обновления.");
            }
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound($"Пользователь с ID {id} не найден.");
            }
            patchDoc.ApplyTo(user);
            if (patchDoc.Operations.Any(op => op.path.Contains("password", StringComparison.OrdinalIgnoreCase)))
            {
                user.Password = PasswordHash.HashPassword(user.Password);
            }
            _context.SaveChanges();
            return Ok(user);
        }

        [HttpGet]
        [Route("/Users/Authenticate")]
        public IActionResult Authenticate(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound($"Пользователь с email {email} не найден.");
            }
            if (!PasswordHash.VerifyPassword(password, user.Password))
            {
                return Unauthorized("Неверный пароль.");
            }
            return Ok(user);
        }

        [HttpGet]
        [Route("/Users/VerifyToken")]
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


        [HttpGet]
        [Route("/Users/ResendToken")]
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

        //
        // Tokens Controller Methods
        //


        [HttpGet]
        [Route("/Tokens")]
        public IActionResult GetTokens()
        {
            var tokens = _context.Tokensses;
            if (tokens == null)
            {
                return NotFound("Токены не найдены.");
            }
            return Ok(tokens);
        }

        [HttpDelete]
        [Route("/Tokens/{id}")]
        public IActionResult DeleteTokens(int id)
        {
            var token = _context.Tokensses.Find(id);
            if (token == null)
            {
                return NotFound($"Токен с ID {id} не найден.");
            }
            _context.Tokensses.Remove(token);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete]
        [Route("/Tokens/All")]
        public IActionResult DeleteAllTokens()
        {
            var tokens = _context.Tokensses.ToList();
            if (!tokens.Any())
            {
                return NotFound("Токены не найдены.");
            }
            _context.Tokensses.RemoveRange(tokens);
            _context.SaveChanges();
            return NoContent();
        }


        //
        // UserInfos Controller Methods
        //


        [HttpGet]
        [Route("/UserInfos")]
        public IActionResult GetAllUsersInfo()
        {
            var users = _context.UserInfos.ToList();
            return Ok(users);
        }

        [HttpGet]
        [Route("/UserInfos/{id}")]
        public IActionResult GetUsersInfoById(int id)
        {
            var user = _context.UserInfos.Find(id);
            if (user == null)
            {
                return NotFound($"Пользователь с ID {id} не найден.");
            }
            return Ok(user);
        }

        [HttpDelete]
        [Route("/UserInfos/{id}")]
        public IActionResult DeleteUserInfos(int id)
        {
            var user = _context.UserInfos.Find(id);
            if (user == null)
            {
                return NotFound($"Пользователь с ID {id} не найден.");
            }
            _context.UserInfos.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }
    }
}