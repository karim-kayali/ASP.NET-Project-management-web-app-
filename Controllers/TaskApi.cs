using AdvancedFinalProject.DTO;
using AdvancedFinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace AdvancedFinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskApi : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskApi(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("displaytasks")]
        public IActionResult Index(int projectId)
        {
            var tasks = _context.tasks.Include(p=>p.Assignee)
                .Where(p => p.ProjectId == projectId);
            return Ok(tasks);
        }



        [HttpPost("addmember")]
        public IActionResult AddMemberToProject( int projectId,   int userId)
        {



            var project = _context.projects.Include(p => p.Members).FirstOrDefault(p => p.ProjectId == projectId);
            var user = _context.users.FirstOrDefault(u => u.UserId == userId);


             
            

            if (!project.Members.Any(m => m.UserId == userId))
            {
                project.Members.Add(user);
                _context.SaveChanges();
            }

            return Ok("Member added successfully.");
        }



        [HttpPost("createtask")]
        public async Task<IActionResult> CreateTask( TaskDTO   dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = new TaskItem
            {
                TaskName = dto.TaskName,
                TaskDescription = dto.TaskDescription,
                TaskPriority = dto.TaskPriority,
                TaskDueDate = dto.TaskDueDate,
                TaskStatus = "New",
                CreatorId= dto.CreatorId,
                AssignedToId = dto.AssignedToId,
                ProjectId=dto.ProjectId
             
            };

            _context.tasks.Add(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }



        [HttpGet("detailedtask")]
        public async Task<IActionResult> DetailsApi( int id)
        {


            var task = await _context.tasks
                .Include(p => p.Creator)
                .Include(p => p.Assignee)
                .FirstOrDefaultAsync(m => m.TaskId == id);


            return Ok(task);
        }


        [HttpDelete("deletetask/{Id}")]
        public async Task<IActionResult> DeleteProject(int Id)
        {
            var task = await _context.tasks.FindAsync(Id);

           
            _context.tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content status code on successful deletion
        }



        [HttpPost("edittask")]
        public async Task<IActionResult> Edit( int taskid, EditTaskDTO dto)
        {

         
            var existingTask = await _context.tasks.FindAsync(taskid);



            existingTask.TaskName = dto.TaskName;
            existingTask.TaskDescription = dto.TaskDescription;
            existingTask.TaskDueDate= dto.TaskDueDate;
            existingTask.TaskPriority = dto.TaskPriority;

         
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("RemoveMember")]
        public IActionResult RemoveMember(int userId, int projectId)
        {
            var project = _context.projects
                .Include(p => p.Members)
                .Include(p => p.Tasks)
                .FirstOrDefault(p => p.ProjectId == projectId);

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            var user = _context.users.Find(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

             
            project.Members.Remove(user);

          
            var task = _context.tasks.FirstOrDefault(t => t.AssignedToId == userId && t.ProjectId == projectId);

            if (task != null)
            {
              
                task.AssignedToId = project.CreatorId;
                task.Assignee = project.Creator;
            }

            
            _context.SaveChanges();

            return Ok();
        }


    }
}


