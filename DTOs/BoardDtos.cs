namespace KanbanBackend.DTOs
{
    public class CreateBoardDto
    {
        public string Title { get; set; } = string.Empty;
    }

    public class CreateColumnDto
    {
        public string Title { get; set; } = string.Empty;
        public int BoardId { get; set; }
        public int Order { get; set; }
    }

    public class CreateCardDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ColumnId { get; set; }
        public int Order { get; set; }
    }

    // Jab card ko drag-drop karke doosre column me / doosri position pe daalte hain
    public class MoveCardDto
    {
        public int CardId { get; set; }
        public int NewColumnId { get; set; }
        public int NewOrder { get; set; }
    }
}
