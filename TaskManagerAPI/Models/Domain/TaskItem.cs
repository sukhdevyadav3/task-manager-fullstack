namespace TaskManagerAPI.Models.Domain
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateOfCompletion { get; set; }

        // Foreign key by username
        public string Username { get; set; }
    }
}
