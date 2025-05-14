using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using Microsoft.EntityFrameworkCore;
using ApiGradeProject.Database;

namespace ApiGradeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRolesController : ControllerBase
    {
        private readonly PostgresContext _context;

        public UserRolesController(PostgresContext context)
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



        [HttpGet("{userId}")]
        public IActionResult GetByUserId(int userId)
        {
            var userRoles = _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToList();

            if (!userRoles.Any())
                return NotFound($"Роли пользователя с ID {userId} не найдены.");

            return Ok(userRoles);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleDto dto)
        {
            if (dto == null)
                return BadRequest("Неверные данные.");

            var existingUserRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == dto.UserId);

            if (existingUserRole != null)
            {
                _context.UserRoles.Remove(existingUserRole);
                await _context.SaveChangesAsync();
            }

            var alreadyAssigned = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == dto.UserId && ur.RoleId == dto.RoleId);

            if (alreadyAssigned)
                return Conflict("Пользователь уже имеет эту роль.");

            var userRole = new UserRole
            {
                UserId = dto.UserId,
                RoleId = dto.RoleId
            };

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Роль успешно изменена." });
        }


        [HttpDelete("id")]
        public async Task<IActionResult> RemoveRole(int userId, int roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null)
                return NotFound("Роль не найдена у пользователя.");

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class UserRoleDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}