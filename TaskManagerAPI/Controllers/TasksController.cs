using Microsoft.AspNetCore.Authorization; // For adding authorization functionality
using Microsoft.AspNetCore.Mvc; // For MVC framework functionality
using TaskManagerAPI.Data; // For database context
using TaskManagerAPI.Models.Domain; // For task-related domain models
using TaskManagerAPI.Models.DTO; // For Data Transfer Objects (DTOs)
using System.Linq; // For LINQ operations

// Controller route and action setup
[Route("api/[controller]")] // Defines the base route for all actions in this controller (e.g., api/tasks)
[ApiController] // Automatically handles model binding, validation, and response formatting
[Authorize]  // Protect this controller with authentication, ensuring only authenticated users can access it
public class TasksController : ControllerBase
{
    private readonly TaskItemDbContext dbContext; // Database context to interact with the database

    // Constructor that initializes dbContext for dependency injection
    public TasksController(TaskItemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET: api/tasks?search=xyz&category=Work&isCompleted=true
    [HttpGet] // GET method to fetch tasks
    public IActionResult GetAll([FromQuery] string? search, [FromQuery] string? category, [FromQuery] bool? isCompleted)
    {
        var username = User.Identity.Name; // Fetch the logged-in user's username

        var query = dbContext.TaskItems.AsQueryable(); // Start with all tasks in the database

        // Filter tasks by username (to ensure users only see their tasks)
        query = query.Where(t => t.Username == username);

        // Add optional filters based on the query parameters
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t => t.Title.Contains(search)); // Search by title
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(t => t.Category == category); // Filter by category
        }

        if (isCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == isCompleted.Value); // Filter by completion status
        }

        var tasks = query.ToList(); // Execute the query and fetch the tasks

        // Map tasks to TaskDto to return only relevant data to the client
        var tasksDto = tasks.Select(task => new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Category = task.Category,
            IsCompleted = task.IsCompleted,
            Username = task.Username,
            DateOfCompletion = task.DateOfCompletion
        }).ToList();

        return Ok(tasksDto); // Return the tasks as a JSON response
    }

    // GET: api/tasks/{id}
    [HttpGet("{id:Guid}")] // GET method to fetch a task by its ID
    public IActionResult GetById([FromRoute] Guid id)
    {
        var username = User.Identity.Name; // Fetch the logged-in user's username

        // Find the task by ID and username (ensures users can't access others' tasks)
        var task = dbContext.TaskItems.FirstOrDefault(t => t.Id == id && t.Username == username);
        if (task == null)
        {
            return NotFound(); // Return a 404 if the task is not found
        }

        // Map the task to TaskDto for the response
        var taskDto = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Category = task.Category,
            IsCompleted = task.IsCompleted,
            Username = task.Username,
            DateOfCompletion = task.DateOfCompletion
        };

        return Ok(taskDto); // Return the task as a JSON response
    }

    // POST: api/tasks
    [HttpPost] // POST method to create a new task
    public IActionResult Create([FromBody] AddTasksRequestDto dto)
    {
        var username = User.Identity.Name; // Fetch the logged-in user's username

        // Create a new TaskItem object using the provided data
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            IsCompleted = dto.IsCompleted,
            DateOfCompletion = dto.DateOfCompletion,
            Username = username // Associate the task with the current user
        };

        // Add the task to the database and save the changes
        dbContext.TaskItems.Add(task);
        dbContext.SaveChanges();

        // Map the task to TaskDto for the response
        var taskDto = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Category = task.Category,
            IsCompleted = task.IsCompleted,
            Username = task.Username,
            DateOfCompletion = task.DateOfCompletion
        };

        // Return a 201 status with the created task data
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, taskDto);
    }

    // PUT: api/tasks/{id}
    [HttpPut("{id:Guid}")] // PUT method to update an existing task
    public IActionResult Update([FromRoute] Guid id, [FromBody] UpdateTaskRequestDto dto)
    {
        var username = User.Identity.Name; // Fetch the logged-in user's username

        // Find the task by ID and username (ensures users can't update others' tasks)
        var task = dbContext.TaskItems.FirstOrDefault(t => t.Id == id && t.Username == username);
        if (task == null)
        {
            return NotFound(); // Return a 404 if the task is not found
        }

        // Update the task properties with the provided data
        task.Title = dto.Title ?? task.Title;
        task.Description = dto.Description ?? task.Description;
        task.Category = dto.Category ?? task.Category;
        task.IsCompleted = dto.IsCompleted;
        task.DateOfCompletion = dto.DateOfCompletion;

        // Save the changes to the database
        dbContext.SaveChanges();

        return NoContent(); // Return a 204 status to indicate the update was successful
    }

    // DELETE: api/tasks/{id}
    [HttpDelete("{id:Guid}")] // DELETE method to remove a task
    public IActionResult Delete([FromRoute] Guid id)
    {
        var username = User.Identity.Name; // Fetch the logged-in user's username

        // Find the task by ID and username (ensures users can't delete others' tasks)
        var task = dbContext.TaskItems.FirstOrDefault(t => t.Id == id && t.Username == username);
        if (task == null)
        {
            return NotFound(); // Return a 404 if the task is not found
        }

        // Remove the task from the database and save the changes
        dbContext.TaskItems.Remove(task);
        dbContext.SaveChanges();

        return NoContent(); // Return a 204 status to indicate the delete was successful
    }
}
