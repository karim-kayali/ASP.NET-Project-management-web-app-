using AdvancedFinalProject.DTO;
using AdvancedFinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
 
namespace AdvancedFinalProject.Controllers
{
    public class RegistrationController : Controller
    {
        private ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

      

        public RegistrationController(ApplicationDbContext context , HttpClient httpClient )
        {
            _context = context;
            _httpClient = httpClient;
           
           
        }



        [HttpGet]
        public IActionResult SignUp()
        {
            return View("SignUp");
        }



        [HttpPost]
        public async Task<IActionResult> SignUp(UserDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            // Send the HTTP POST request to the SignUp API
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7159/api/userapi/signup", dto);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var token = json["token"];

                HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return RedirectToAction("Index", "Projects");
            }

            else
            {

                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(dto);  // Return the same view with error 
            }

        }


        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }


        /*    [HttpPost]

            public async Task<IActionResult> LogIn(UserDTO dto)
            {


                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                {
                    ModelState.AddModelError(string.Empty, "Email and Password are required.");
                    return View();
                }

                var existingUser = await _context.users
                    .FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (existingUser == null)
                { 
                    ModelState.AddModelError(string.Empty, "Email not found.");
                    return View();
                }

                if (existingUser.Password != dto.Password)
                {

                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return View();
                }

                HttpContext.Session.SetInt32("UserId", existingUser.UserId); // Or SetString if it's a Guid or string
                return RedirectToAction("Index", "Projects");


            }*/





        [HttpPost]
        public async Task<IActionResult> LogIn(LoginUserDTO dto)
        {
             

           
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Email and password are required.");
            }

         
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7159/api/userapi/login", dto);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var token = json["token"];

                
                HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return RedirectToAction("Index", "Projects");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(dto);  // Return the same view with error 
            }
        }




    }
}
