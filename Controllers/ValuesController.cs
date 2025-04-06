using AdvancedFinalProject.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

 

namespace AdvancedFinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
       
        public class ValuesController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public ValuesController(ApplicationDbContext context)
            {
                _context = context;
            }

            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] User user)
            {
                if (user == null)
                {
                    return BadRequest("User data is required");
                }

                if (ModelState.IsValid)
                {
                    // Check if the email already exists
                    var existingUser = await _context.users
                        .FirstOrDefaultAsync(u => u.Email == user.Email);

                    if (existingUser != null)
                    {
                        return BadRequest("A user with this email already exists.");
                    }

                   
                    await _context.users.AddAsync(user);
                    await _context.SaveChangesAsync();

                   
                    return Ok(new { message = "User registered successfully" });
                }

                return BadRequest(ModelState);
            }



       
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest("Login data is required");
            }

            // Check if the email exists
            var user = await _context.users
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Check if the password matches
            if (user.Password != loginRequest.Password)
            {
                return Unauthorized("Invalid email or password.");
            }

          

            return Ok(new { message = "Login successful", userId = user.UserId });
        }
    }
}
    

