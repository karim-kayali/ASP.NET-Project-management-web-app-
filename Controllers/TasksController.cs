using AdvancedFinalProject.DTO;
using AdvancedFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace AdvancedFinalProject.Controllers
{
    public class TasksController : Controller
    {

        private ApplicationDbContext _context;
        private readonly HttpClient _httpClient;



        public TasksController(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;


        }

        [HttpGet]
        public async Task<IActionResult> Index(int projectId)
        {


            var token = Request.Cookies["jwt"];

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);


            int userId = int.Parse(userIdClaim.Value);

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


            var response = await _httpClient.GetAsync($"https://localhost:7159/api/taskapi/displaytasks?projectId={projectId}");



            var jsonString = await response.Content.ReadAsStringAsync();
            var tasks = JsonConvert.DeserializeObject<List<TaskItem>>(jsonString);

            var project = _context.projects.FirstOrDefault(p => p.ProjectId == projectId);

            if (project == null)
            {
                 
                return NotFound("Project not found.");
            }

            var creatorId = project.CreatorId;


            ViewData["UserId"] = userId;

            ViewBag.CreatorId = creatorId;


            ViewBag.ProjectId = projectId;

            return View(tasks);
        }



        [HttpGet]
        public IActionResult AddMember(int projectId)
        {


            var token = Request.Cookies["jwt"];

            _httpClient.DefaultRequestHeaders.Authorization =
              new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


            ViewBag.ProjectId = projectId;

            var project = _context.projects.Include(c=>c.Creator)
                   .Include(p => p.Members)
                   .FirstOrDefault(p => p.ProjectId == projectId);



            ViewBag.Members = project.Members;
            ViewBag.Creator = project.Creator;


            return View(); 
        }


        [HttpPost]
        public async Task<IActionResult> AddMember(string userName, int projectId)
        {
            var project = _context.projects.Include(c => c.Creator)
                     .Include(p => p.Members)
                     .FirstOrDefault(p => p.ProjectId == projectId);

            ViewBag.ProjectId = projectId;




            ViewBag.Members = project.Members;
            ViewBag.Creator = project.Creator;

            var token = Request.Cookies["jwt"];

            _httpClient.DefaultRequestHeaders.Authorization =
              new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);



            var user = _context.users.FirstOrDefault(u => u.UserName == userName);
 
            if (user==null)
            {
                ModelState.AddModelError("", "Failed to add member.");

               

                return View();
            }
            else if (project.Members.Find(u => u.UserName == userName) != null || project.Creator.UserName.Equals(  userName))

            {
                ModelState.AddModelError("", "Member already in project.");



                return View();
            }

         

            var response = await _httpClient.PostAsync($"https://localhost:7159/api/taskapi/addmember?projectId={projectId}&userId={user.UserId}", null);

                if (response.IsSuccessStatusCode)
                {

                var project1 = _context.projects.Include(c => c.Creator)
                 .Include(p => p.Members)
                 .FirstOrDefault(p => p.ProjectId == projectId);

                ViewBag.ProjectId = projectId;


                ViewBag.Members = project1.Members;
                ViewBag.Creator = project1.Creator;

                TempData["SuccessMessage"] = "Member added successfully!";
                return View();

                 
            }
                else
                {
                ModelState.AddModelError("", "Failed to add member.");
               
                return View();

            }

        }

        [HttpGet]
        public async Task<IActionResult> Create(int projectId)
        {
          
            var project = await _context.projects
                .Include(p => p.Members).Include(p=>p.Creator)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null)
            {
                return NotFound();
            }

           
            ViewBag.Users = project.Members;

            ViewBag.CreatorId = project.Creator;

            ViewBag.ProjectId = projectId;

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create(TaskDTO dto)
        {


            var token = Request.Cookies["jwt"];

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);


            int userId = int.Parse(userIdClaim.Value);

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


       
          
         

            dto.CreatorId = userId;


            var response = await _httpClient.PostAsJsonAsync("https://localhost:7159/api/taskapi/createtask", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", new { projectId = dto.ProjectId });

            ModelState.AddModelError("", "Error creating task");
            ViewBag.Users = await _context.users.ToListAsync(); // refill dropdown again
            return View(dto);
        }


        public async Task<IActionResult> Details(int id)
        {

            var token = Request.Cookies["jwt"];


            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


            var response = await _httpClient.GetAsync($"https://localhost:7159/api/taskapi/detailedtask?id={id}");

            if (!response.IsSuccessStatusCode)
            {
                
                var errorContent = await response.Content.ReadAsStringAsync();
                return Content($"API Error: {response.StatusCode}\n{errorContent}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var task = JsonConvert.DeserializeObject<TaskItem>(jsonString);

            return View(task);
        }




        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            var task = await _context.tasks
               
                .FirstOrDefaultAsync(p => p.TaskId == id);



            return View(task);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var token = Request.Cookies["jwt"];


            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


            var task = await _context.tasks.FirstOrDefaultAsync(t => t.TaskId == id);


            int projectId = task.ProjectId;  



            var response = await _httpClient.DeleteAsync($"https://localhost:7159/api/taskapi/deletetask/{id}");

          

            return RedirectToAction("Index", new { projectId = projectId });
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
             

            var task = await _context.tasks.FindAsync(id);
            
            
            return View(task);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int taskid, EditTaskDTO dto)
        {

            var token = Request.Cookies["jwt"];


            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);



            var taskData = new EditTaskDTO
            {
                 TaskName = dto.TaskName,
            TaskDescription = dto.TaskDescription,
            TaskDueDate = dto.TaskDueDate,
            TaskPriority = dto.TaskPriority

        };


            var content = new StringContent(JsonConvert.SerializeObject(taskData), Encoding.UTF8, "application/json");


            var response = await _httpClient.PostAsync($"https://localhost:7159/api/taskapi/edittask?taskid={taskid}", content);

            if (response.IsSuccessStatusCode)
            {
                var task = await _context.tasks.FindAsync(taskid);

                var projectid = task.ProjectId;

                return RedirectToAction("Index", "Tasks", new { projectId = projectid });
            }
            else
            {

                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int TaskId, string TaskStatus)
        {
            var task = await _context.tasks.FindAsync(TaskId);
 

            task.TaskStatus = TaskStatus;
            await _context.SaveChangesAsync();

            
            return RedirectToAction("Index", new { projectId = task.ProjectId });
        }


        [HttpPost]
        public async Task<IActionResult> RemoveMember(int userId, int projectId)
        {

            var token = Request.Cookies["jwt"];


            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);



            var response = await _httpClient.PostAsync($"https://localhost:7159/api/taskapi/RemoveMember?userId={userId}&projectId={projectId}", null);

            ViewBag.ProjectId = projectId;

            var project = _context.projects.Include(c => c.Creator)
                   .Include(p => p.Members)
                   .FirstOrDefault(p => p.ProjectId == projectId);



            ViewBag.Members = project.Members;
            ViewBag.Creator = project.Creator;

            return View("AddMember");
        }



    }
}
