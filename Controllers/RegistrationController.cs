using AdvancedFinalProject.DTO;
using AdvancedFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace AdvancedFinalProject.Controllers
{
    public class RegistrationController : Controller
    {
        private ApplicationDbContext _context;
        

        public RegistrationController(ApplicationDbContext context )
        {
            _context = context;
           
        }



        [HttpGet]
        public IActionResult SignUp()
        {
            return View("SignUp");
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserDTO dto)
        {
            if (!ModelState.IsValid)
            {
               
                return View( );  
            }

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Password = dto.Password, // Ideally hash this
            };

            // Check if the username already exists
            var existingUserByUsername = await _context.users
                .FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (existingUserByUsername != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already taken");
                return View( );   
            }

           
            var existingUserByEmail = await _context.users
                .FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already taken");
                return View( );   
            }


            HttpContext.Session.SetInt32("UserId", user.UserId);  

            
            _context.Add(user);
            await _context.SaveChangesAsync();

            return View("~/Views/Projects/Index.cshtml");
            
        }


        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }


        [HttpPost]
      
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


        }

    }
}
