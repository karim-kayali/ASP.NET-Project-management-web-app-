using AdvancedFinalProject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvancedFinalProject.Models;

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
                CreatorId = dto.CreatorId
            };


            _context.Add(project);
            await _context.SaveChangesAsync();

            return Ok( );
        }
    }
}
