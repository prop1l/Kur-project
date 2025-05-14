using Microsoft.AspNetCore.Mvc;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Database;
using Microsoft.EntityFrameworkCore;

namespace ApiGradeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly PostgresContext _context;

        public RatingsController(PostgresContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Ratings.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var rating = _context.Ratings.Find(id);
            return rating == null ? NotFound($"Оценка с ID {id} не найдена.") : Ok(rating);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Rating newRating)
        {
            if (newRating == null) return BadRequest("Неверные данные.");
            await _context.Ratings.AddAsync(newRating);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newRating.RatingId }, newRating);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Rating updatedRating)
        {
            if (updatedRating == null || id != updatedRating.RatingId) return BadRequest("Неверные данные.");

            var existing = await _context.Ratings.FindAsync(id);
            if (existing == null) return NotFound($"Оценка с ID {id} не найдена.");

            _context.Entry(existing).CurrentValues.SetValues(updatedRating);
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null) return NotFound($"Оценка с ID {id} не найдена.");

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}