using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdvancedFinalProject;
using AdvancedFinalProject.Models;
using Microsoft.AspNetCore.Identity;

namespace AdvancedFinalProject.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var projects = await _context.projects
                .Include(p => p.Creator)
                .Include(p => p.Members)
                .Where(p => p.CreatorId == userId || p.Members.Any(m => m.UserId == userId))
                .ToListAsync();

            return View(projects);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.projects
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string ProjectTitle, string? ProjectDescription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("LogIn", "Registration");
            }

            Project project = new Project();
            project.ProjectDescription = ProjectDescription;
            project.ProjectTitle = ProjectTitle;
            project.CreatorId = userId.Value;

            _context.Add(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.projects.FindAsync(id);
            if (project == null) return NotFound();

            return View(project);
        }
        [HttpPost]
        
        public async Task<IActionResult> Edit(int ProjectId, string ProjectTitle, string ProjectDescription)
        {
            var existingProject = await _context.projects.FindAsync(ProjectId);
          
            existingProject.ProjectTitle = ProjectTitle;
            existingProject.ProjectDescription = ProjectDescription;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/Delete/1
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
             
            var project = await _context.projects
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

          

            return View(project);
        }
         
        [HttpPost ]
        
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.projects.FindAsync(id);
          

            _context.projects.Remove(project);  
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the Index page after deletion
        }



    }
}
