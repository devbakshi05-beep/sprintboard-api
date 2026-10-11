namespace KanbanBackend.Models
{
    // Ek Board = ek project/workspace, jisme multiple columns hote hain
    public class Board
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key — ye board kis user ka hai
        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Ek board ke multiple columns (To Do, In Progress, Done)
        public List<BoardColumn> Columns { get; set; } = new();
    }
}
