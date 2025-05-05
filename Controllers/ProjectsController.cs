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
using Newtonsoft.Json;
using System.Text;
using Microsoft.CodeAnalysis;
using Project = AdvancedFinalProject.Models.Project;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NuGet.Common;

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
            var token = Request.Cookies["jwt"];

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);


            int userId = int.Parse(userIdClaim.Value);  
 
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


            var response = await _httpClient.GetAsync($"https://localhost:7159/api/projectapi/displayprojects?userId={userId}");

            var jsonString = await response.Content.ReadAsStringAsync();
            var projects = JsonConvert.DeserializeObject<List<Project>>(jsonString);

            ViewData["UserId"] = userId;

            return View(projects);
        }

        public async Task<IActionResult> Details(int id)
        {
            var token = Request.Cookies["jwt"];

            _httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


            var response = await _httpClient.GetAsync($"https://localhost:7159/api/projectapi/detailedproject?id={id}");
 

            var jsonString = await response.Content.ReadAsStringAsync();
            var project = JsonConvert.DeserializeObject<Project>(jsonString); 

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

           

        
         

            var token = Request.Cookies["jwt"];

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);


            int userId = int.Parse(userIdClaim.Value);

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            dto.CreatorId= userId;


            var project = _context.projects
     .FirstOrDefault(p => p.ProjectTitle == dto.ProjectTitle && p.CreatorId == dto.CreatorId);

            if (project != null)
            {
                ModelState.AddModelError("", "A project with this title already exists.");
                return View(dto);
            }


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
        public async Task<IActionResult> Edit(int projectId,EditProjectDTO dto)
        {   
            var projectData = new EditProjectDTO
            {
                ProjectTitle = dto.ProjectTitle,   
                ProjectDescription = dto.ProjectDescription   
            };
 
 
            var content = new StringContent(JsonConvert.SerializeObject(projectData), Encoding.UTF8, "application/json");


            var token = Request.Cookies["jwt"];

            _httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);




            var response = await _httpClient.PostAsync($"https://localhost:7159/api/projectapi/editproject?projectId={projectId}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Projects");
            }
            else
            {
              
                return View();
            }
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
             
            var project = await _context.projects
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

          

            return View(project);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var token = Request.Cookies["jwt"];

            _httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);



            var response = await _httpClient.DeleteAsync($"https://localhost:7159/api/projectapi/deleteproject?Id={id}");


            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                
                return View("Error", errorMessage);   
            }

            
            return RedirectToAction(nameof(Index));
        }





    }
}
