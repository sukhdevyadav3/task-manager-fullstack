namespace TaskManagerAPI.Models.DTO  // Namespace for Data Transfer Objects related to the Task Manager API.
{
    public class AddTasksRequestDto  // This DTO class is used when receiving data for creating a new task.
    {
        public string Title { get; set; }  // The title of the task

        public string Description { get; set; }  // A detailed description of the task

        public string Category { get; set; }  // The category of the task

        public bool IsCompleted { get; set; }  // A boolean indicating whether the task is completed or not.

        public DateTime? DateOfCompletion { get; set; }  // The date when the task was or will be completed. Nullable as not all tasks may have a completion date at the time of creation.
    }
}
