using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Database;

namespace ApiGradeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly PostgresContext _context;

        public SubjectsController(PostgresContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Subjects.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var subject = _context.Subjects.Find(id);
            return subject == null ? NotFound($"Предмет с ID {id} не найден.") : Ok(subject);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Subject newSubject)
        {
            if (newSubject == null) return BadRequest("Неверные данные.");
            await _context.Subjects.AddAsync(newSubject);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newSubject.SubjectId }, newSubject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Subject updatedSubject)
        {
            if (updatedSubject == null || id != updatedSubject.SubjectId) return BadRequest("Неверные данные.");

            var existing = await _context.Subjects.FindAsync(id);
            if (existing == null) return NotFound($"Предмет с ID {id} не найден.");

            _context.Entry(existing).CurrentValues.SetValues(updatedSubject);
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound($"Предмет с ID {id} не найден.");

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}