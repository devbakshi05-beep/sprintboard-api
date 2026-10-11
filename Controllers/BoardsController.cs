using System.Security.Claims;
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
    [Authorize] // Is controller ke saare endpoints ke liye valid JWT token chahiye
    public class BoardsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public BoardsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Token se logged-in user ki ID nikalne ka helper
        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

        // GET /api/boards — logged-in user ke saare boards, columns aur cards ke saath
        [HttpGet]
        public async Task<ActionResult<List<Board>>> GetBoards()
        {
            var userId = GetUserId();

            var boards = await _db.Boards
                .Where(b => b.UserId == userId)
                .Include(b => b.Columns.OrderBy(c => c.Order))
                    .ThenInclude(c => c.Cards.OrderBy(card => card.Order))
                .ToListAsync();

            return Ok(boards);
        }

        // POST /api/boards — naya board banao
        [HttpPost]
        public async Task<ActionResult<Board>> CreateBoard(CreateBoardDto dto)
        {
            var userId = GetUserId();

            var board = new Board
            {
                Title = dto.Title,
                UserId = userId
            };

            _db.Boards.Add(board);
            await _db.SaveChangesAsync();

            return Ok(board);
        }

        // DELETE /api/boards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            var userId = GetUserId();
            var board = await _db.Boards.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (board == null) return NotFound();

            _db.Boards.Remove(board);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
