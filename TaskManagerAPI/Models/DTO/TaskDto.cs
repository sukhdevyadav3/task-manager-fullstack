namespace TaskManagerAPI.Models.DTO
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateOfCompletion { get; set; }
        public string Username { get; set; }
    }
}
