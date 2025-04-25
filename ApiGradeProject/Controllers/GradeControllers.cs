using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Script;
using Microsoft.AspNetCore.JsonPatch;


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

        // GET: /Groups - Получить все группы
        [HttpGet]
        [Route("/Groups")]
        public IActionResult GetAllGroups()
        {
            var groups = _context.Groups.ToList();
            return Ok(groups);
        }

        // GET: /Groups/{id} - Получить группу по ID
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

        // POST: /Groups - Добавить новую группу
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


        // DELETE: /Groups/{id} - Удалить группу по ID
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

        // GET: /Users - Получить всех пользователей
        [HttpGet]
        [Route("/Users")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }


        // POST: /Users - Регистрация нового пользователя и отправка токена подтверждения
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

            // TODO: Send email with token to the user (e.g., using SMTP or a third-party service)
            // Example: SendEmail(newUser.Email, "Confirm your email", $"Click here to confirm: http://localhost:5172/Users/ConfirmEmail?token={token}");

            return Ok(new { Message = "Регистрация успешна. Пожалуйста, подтвердите ваш email.", Token = token });
        }


        // GET: /Users/{id} - Получить пользователя по ID
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

        // DELETE: /Users/{id} - Удалить пользователя по ID
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

        
        // PUT: /Users/{id} - Полное обновление данных пользователя
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

        
        // PATCH: /Users/{id} - Частичное обновление данных пользователя
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

        // GET: /Users/Authenticate?email=example@example.com&password=12345 - Авторизация пользователя
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
            var token = _context.Tokensses.FirstOrDefault(u => u.UsersId == userId && u.TokenValue == verifyToken);

            if( token == null)
            {
                return NotFound("No tokens");
            }

            if(token.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest("Срок действия токена истек.");
            }

            var user = _context.Users.Find(userId);
            user.IsEmailConfirmed = true;
            _context.Tokensses.Remove(token);
            _context.SaveChanges();

            return Ok(new { Message = "Почта потверждена!" });
        }




        // GET: /UsersInfo - Получить данные пользователей
        [HttpGet]
        [Route("/UserInfos")]
        public IActionResult GetAllUsersInfo()
        {
            var users = _context.UserInfos.ToList();
            return Ok(users);
        }


        // GET: /UserInfos/{id} - Получить данные по ID
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


        // DELETE: /UserInfos/{id} - Удалить пользователя по ID
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


        // GET: /Tokens - Получить все токены
        [HttpGet]
        [Route("/Tokens")]
        public IActionResult GetTokens()
        {
            var token = _context.Tokensses;
            if (token == null)
            {
                return NotFound($"Ничего не найдено");
            }
            return Ok(token);
        }


        // DELETE: /Tokens/{id} - Удалить токен по ID
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


        // DELETE: /Tokens/All - Удалить все токены
        [HttpDelete]
        [Route("/Tokens/All")]
        public IActionResult DeleteAllTokens()
        {
            var tokens = _context.Tokensses.ToList();

            if (tokens == null || !tokens.Any())
            {
                return NotFound("Токены не найдены.");
            }

            _context.Tokensses.RemoveRange(tokens);
            _context.SaveChanges();

            return NoContent();
        }
    }
}