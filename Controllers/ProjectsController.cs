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
using AdvancedFinalProject.DTO;
using System.Net.Http;

namespace AdvancedFinalProject.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        

        public ProjectsController(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;


        }


        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("SignUp", "Registration");
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
          

            return View(project);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectDTO dto)
        {

            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("LogIn", "Registration");
            }

            dto.CreatorId = userId.Value; 

           
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7159/api/projectapi/createproject", dto);

            if (response.IsSuccessStatusCode)
            { 

                return RedirectToAction("Index", "Projects");
            }

            else
            {

                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(dto);   
            }
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
        
        public async Task<IActionResult> Edit(int ProjectId, ProjectDTO dto)
        {
            var existingProject = await _context.projects.FindAsync(ProjectId);
          
            existingProject.ProjectTitle = dto.ProjectTitle;
            existingProject.ProjectDescription = dto.ProjectDescription;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    
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

            return RedirectToAction(nameof(Index));  
        }



    }
}
