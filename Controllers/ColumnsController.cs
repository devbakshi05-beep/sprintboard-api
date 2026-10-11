using KanbanBackend.Data;
using KanbanBackend.DTOs;
using KanbanBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ColumnsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ColumnsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST /api/columns — board me naya column add karo (To Do / In Progress / Done)
        [HttpPost]
        public async Task<ActionResult<BoardColumn>> CreateColumn(CreateColumnDto dto)
        {
            var column = new BoardColumn
            {
                Title = dto.Title,
                BoardId = dto.BoardId,
                Order = dto.Order
            };

            _db.Columns.Add(column);
            await _db.SaveChangesAsync();

            return Ok(column);
        }

        // DELETE /api/columns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(int id)
        {
            var column = await _db.Columns.FindAsync(id);
            if (column == null) return NotFound();

            _db.Columns.Remove(column);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
