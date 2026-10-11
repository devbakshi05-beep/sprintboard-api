namespace KanbanBackend.Models
{
    // Ek Column = board ke andar ek stage (To Do / In Progress / Done)
    public class BoardColumn
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // Order matter karta hai — columns ko left-to-right sahi sequence me dikhane ke liye
        public int Order { get; set; }

        public int BoardId { get; set; }
        public Board? Board { get; set; }

        // Ek column ke andar multiple cards (tasks)
        public List<TaskCard> Cards { get; set; } = new();
    }
}
