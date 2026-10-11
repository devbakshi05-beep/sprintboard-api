namespace KanbanBackend.Models
{
    // Ek Card = ek task jo user drag-drop karega columns ke beech
    public class TaskCard
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Card ka position uske column ke andar (drag-drop reorder ke liye zaroori)
        public int Order { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Ye card kis column me hai
        public int ColumnId { get; set; }
        public BoardColumn? Column { get; set; }
    }
}
