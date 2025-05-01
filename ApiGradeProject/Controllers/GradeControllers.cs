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
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
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
            var token = _context.Tokensses.FirstOrDefault(u => u.UsersId == userId && u.TokenValue == verifyToken);
            if (token == null)
            {
                return NotFound("Токен не найден.");
            }
            if (token.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest("Срок действия токена истек.");
            }
            var user = _context.Users.Find(userId);
            user.IsEmailConfirmed = true;
            _context.Tokensses.Remove(token);
            _context.SaveChanges();
            return Ok(new { Message = "Почта подтверждена!" });
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