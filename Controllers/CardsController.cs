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
    public class CardsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CardsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST /api/cards — naya task/card banao
        [HttpPost]
        public async Task<ActionResult<TaskCard>> CreateCard(CreateCardDto dto)
        {
            var card = new TaskCard
            {
                Title = dto.Title,
                Description = dto.Description,
                ColumnId = dto.ColumnId,
                Order = dto.Order
            };

            _db.Cards.Add(card);
            await _db.SaveChangesAsync();

            return Ok(card);
        }

        // PUT /api/cards/move — ye sabse important endpoint hai: jab user card ko drag-drop karta hai
        [HttpPut("move")]
        public async Task<IActionResult> MoveCard(MoveCardDto dto)
        {
            var card = await _db.Cards.FindAsync(dto.CardId);
            if (card == null) return NotFound();

            // Card ko naye column me shift karo aur uski nayi position (Order) set karo
            card.ColumnId = dto.NewColumnId;
            card.Order = dto.NewOrder;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /api/cards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _db.Cards.FindAsync(id);
            if (card == null) return NotFound();

            _db.Cards.Remove(card);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
