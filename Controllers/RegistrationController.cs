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
        public async Task<IActionResult> SignUp(User user)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is invalid, return the view with the user model and errors
                return View(user);  // Pass the user model to the view to display the validation errors
            }

            // Check if the username already exists
            var existingUserByUsername = await _context.users
                .FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (existingUserByUsername != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already taken");
                return View(user);  // Return the user model with the error message
            }

            // Check if the email already exists
            var existingUserByEmail = await _context.users
                .FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already taken");
                return View(user);  // Return the user model with the error message
            }

            // If all checks pass, add the user
            _context.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");   // Redirect to Dashboard after successful sign-up
        }


        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }


        [HttpPost]
      
        public async Task<IActionResult> LogIn(string email, string password)
        {
             

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and Password are required.");
                return View();
            }

            var existingUser = await _context.users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser == null)
            { 
                ModelState.AddModelError(string.Empty, "Email not found.");
                return View();
            }

            if (existingUser.Password != password)
            {
                
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return View();
            }

            return View("Dasboard");


        }

    }
}
