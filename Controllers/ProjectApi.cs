using AdvancedFinalProject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvancedFinalProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace AdvancedFinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectApi : ControllerBase
    {


        private readonly ApplicationDbContext _context;

        public ProjectApi(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost ("createproject")]
       
        public async Task<IActionResult> Create(ProjectDTO dto)
        { 
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            
        
           
            Project project = new Project
            {
                ProjectDescription = dto.ProjectDescription,
                ProjectTitle = dto.ProjectTitle,
                CreatorId = dto.CreatorId,
                   
            };


            _context.Add(project);
            await _context.SaveChangesAsync();

            return Ok( );
        }



        [HttpGet("displayprojects")]
        public async Task<IActionResult> GetProjects(  int userId)
        {
            var user = await _context.users.FirstOrDefaultAsync(p => p.UserId == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var projects = await _context.projects
        .Include(p => p.Creator)  
        .Where(p => p.CreatorId == userId || p.Members.Any(m => m.UserId == userId))
        .ToListAsync();

            return Ok(projects);
        }


        [HttpGet("detailedproject")]
        public async Task<IActionResult> DetailsApi(  int id)
        {
             

            var project = await _context.projects
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(m => m.ProjectId == id);


            return Ok(project);
        }




        [HttpPost("editproject")]
        public async Task<IActionResult> Edit(  int projectId, EditProjectDTO dto)
        {
            var existingProject = await _context.projects.FindAsync(projectId);

            if (existingProject == null)
                return NotFound();

            existingProject.ProjectTitle = dto.ProjectTitle;
            existingProject.ProjectDescription = dto.ProjectDescription;

            await _context.SaveChangesAsync();

            return Ok();
        }





        [HttpDelete("deleteproject")]
        public async Task<IActionResult> DeleteProject(  int Id)
        {
            var project = await _context.projects.FindAsync(Id);

            if (project == null)
            {
                return NotFound(new { message = "Project not found." });
            }

            _context.projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(); 
        }





    }
}
