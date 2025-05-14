using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Database;

namespace ApiGradeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly PostgresContext _context;

        public TeachersController(PostgresContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Teachers.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var teacher = _context.Teachers.Find(id);
            return teacher == null ? NotFound($"Преподаватель с ID {id} не найден.") : Ok(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Teacher newTeacher)
        {
            if (newTeacher == null) return BadRequest("Неверные данные.");
            await _context.Teachers.AddAsync(newTeacher);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newTeacher.TeacherId }, newTeacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Teacher updatedTeacher)
        {
            if (updatedTeacher == null || id != updatedTeacher.TeacherId) return BadRequest("Неверные данные.");

            var existing = await _context.Teachers.FindAsync(id);
            if (existing == null) return NotFound($"Преподаватель с ID {id} не найден.");

            _context.Entry(existing).CurrentValues.SetValues(updatedTeacher);
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound($"Преподаватель с ID {id} не найден.");

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}