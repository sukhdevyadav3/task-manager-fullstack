using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models.Domain;
using TaskManagerAPI.Models.DTO;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
[Authorize]  // Protect this controller with authentication
public class TasksController : ControllerBase
{
    private readonly TaskItemDbContext dbContext;
    public TasksController(TaskItemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET: api/tasks?search=xyz&category=Work&isCompleted=true
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search, [FromQuery] string? category, [FromQuery] bool? isCompleted)
    {
        var username = User.Identity.Name;

        var query = dbContext.TaskItems.AsQueryable();

        query = query.Where(t => t.Username == username);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t => t.Title.Contains(search));
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(t => t.Category == category);
        }

        if (isCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == isCompleted.Value);
        }

        var tasks = query.ToList();

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

        return Ok(tasksDto);
    }

    [HttpGet("{id:Guid}")]
    public IActionResult GetById([FromRoute] Guid id)
    {
        var username = User.Identity.Name;

        var task = dbContext.TaskItems.FirstOrDefault(t => t.Id == id && t.Username == username);
        if (task == null)
        {
            return NotFound();
        }

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

        return Ok(taskDto);
    }

    [HttpPost]
    public IActionResult Create([FromBody] AddTasksRequestDto dto)
    {
        var username = User.Identity.Name;

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            IsCompleted = dto.IsCompleted,
            DateOfCompletion = dto.DateOfCompletion,
            Username = username
        };

        dbContext.TaskItems.Add(task);
        dbContext.SaveChanges();

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

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, taskDto);
    }

    [HttpPut("{id:Guid}")]
    public IActionResult Update([FromRoute] Guid id, [FromBody] UpdateTaskRequestDto dto)
    {
        var username = User.Identity.Name;

        var task = dbContext.TaskItems.FirstOrDefault(t => t.Id == id && t.Username == username);
        if (task == null)
        {
            return NotFound();
        }

        task.Title = dto.Title ?? task.Title;
        task.Description = dto.Description ?? task.Description;
        task.Category = dto.Category ?? task.Category;
        task.IsCompleted = dto.IsCompleted;
        task.DateOfCompletion = dto.DateOfCompletion;

        dbContext.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        var username = User.Identity.Name;

        var task = dbContext.TaskItems.FirstOrDefault(t => t.Id == id && t.Username == username);
        if (task == null)
        {
            return NotFound();
        }

        dbContext.TaskItems.Remove(task);
        dbContext.SaveChanges();

        return NoContent();
    }
}
