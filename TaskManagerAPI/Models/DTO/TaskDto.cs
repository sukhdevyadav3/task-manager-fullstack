namespace TaskManagerAPI.Models.DTO  // Namespace for Data Transfer Objects related to the Task Manager API.
{
    public class TaskDto  // This DTO class is used to represent a task with all its details, typically sent back in API responses.
    {
        public Guid Id { get; set; }  // A unique identifier for the task. Typically used as a primary key in the database.

        public string Title { get; set; }  // The title of the task

        public string Description { get; set; }  // A detailed description of the task

        public string Category { get; set; }  // The category of the task
        public bool IsCompleted { get; set; }  // A boolean indicating whether the task is completed or not.

        public DateTime? DateOfCompletion { get; set; }  // The date when the task was or will be completed. Nullable as not all tasks may have a completion date at the time of creation.

        public string Username { get; set; }  // The username of the user who created the task, providing a way to link tasks to specific users.
    }
}
