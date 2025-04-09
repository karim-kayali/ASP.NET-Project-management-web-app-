using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AdvancedFinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvancedFinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectApi : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ProjectApi(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("{id}")] // id = UserId
        public async Task<IActionResult> Create(int id, [FromBody] Project project)
        {
            if (project == null)
            {
                return BadRequest("Project data is null.");
            }

            // Ensure the user (creator) exists
            var creator = await _context.users.FindAsync(id); // Fetch the creator user

            if (creator == null)
            {
                return NotFound($"User with id {id} not found.");
            }

            // Create a new project and associate the creator
            var projectVar = new Project
            {
                ProjectTitle = project.ProjectTitle,
                ProjectDescription = project.ProjectDescription,
                CreatorId = id,  // Set the CreatorId directly
                Creator = creator // Set the Creator navigation property (optional)
            };

            // Add project to the context and save changes
            _context.projects.Add(projectVar);
            await _context.SaveChangesAsync();

            // Return created project with the location of the new resource
            return CreatedAtAction(nameof(Create), new { id = projectVar.ProjectId }, projectVar);
        }



    }
}
