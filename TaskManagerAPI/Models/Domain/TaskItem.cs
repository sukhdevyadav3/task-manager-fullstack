namespace TaskManagerAPI.Models.Domain  // Namespace for the domain models related to the Task Manager API.
{
    public class TaskItem  // Define the TaskItem class, which represents a task in the task management system.
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // Unique identifier for each task. The default value is generated as a new GUID.

        public string Title { get; set; }  // The title of the task

        public string Description { get; set; }  // A detailed description of the task 

        public string Category { get; set; }  // The category of the task 

        public bool IsCompleted { get; set; }  // A boolean indicating whether the task is completed or not.

        public DateTime? DateOfCompletion { get; set; }  // Optional property to store the date when the task was completed. Nullable to handle incomplete tasks.

        // Foreign key by username
        public string Username { get; set; }  // The username of the person assigned to the task. This is used as a foreign key.
    }
}
