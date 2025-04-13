namespace TaskManagerAPI.Models.DTO  // Namespace for Data Transfer Objects related to the Task Manager API.
{
    public class UpdateTaskRequestDto  // This DTO is used to represent the data that can be updated in an existing task.
    {
        public string Title { get; set; }  // The updated title for the task. If provided, this will replace the current title of the task.

        public string Description { get; set; }  // The updated description for the task. If provided, this will replace the current description of the task.

        public string Category { get; set; }  // The updated category of the task. If provided, this will replace the current category.

        public bool IsCompleted { get; set; }  // The updated completion status of the task. If provided, this will replace the current completion status.

        public DateTime? DateOfCompletion { get; set; }  // The updated date when the task is completed (if provided). Nullable because it may not always be updated.
    }
}
