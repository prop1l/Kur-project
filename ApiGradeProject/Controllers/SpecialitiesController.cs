using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Database;

namespace ApiGradeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialitiesController : ControllerBase
    {
        private readonly PostgresContext _context;

        public SpecialitiesController(PostgresContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Specialities.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var speciality = _context.Specialities.Find(id);
            return speciality == null ? NotFound($"Специальность с ID {id} не найдена.") : Ok(speciality);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Speciality newSpeciality)
        {
            if (newSpeciality == null) return BadRequest("Неверные данные.");
            await _context.Specialities.AddAsync(newSpeciality);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newSpeciality.SpecialityId }, newSpeciality);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Speciality updatedSpeciality)
        {
            if (updatedSpeciality == null || id != updatedSpeciality.SpecialityId) return BadRequest("Неверные данные.");

            var existing = await _context.Specialities.FindAsync(id);
            if (existing == null) return NotFound($"Специальность с ID {id} не найдена.");

            _context.Entry(existing).CurrentValues.SetValues(updatedSpeciality);
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var speciality = await _context.Specialities.FindAsync(id);
            if (speciality == null) return NotFound($"Специальность с ID {id} не найдена.");

            _context.Specialities.Remove(speciality);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}