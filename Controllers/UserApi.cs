using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AdvancedFinalProject.Models;
using Microsoft.EntityFrameworkCore;
using AdvancedFinalProject.DTO;

namespace AdvancedFinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApi : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserApi(ApplicationDbContext context)
        {
            _context = context;
        }

         
        [HttpPost("signup")]
        public async Task<IActionResult> SignUpApi( UserDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest("Invalid user data.");
            }

            // Check if username already exists
            var existingUserByUsername = await _context.users
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName);
            if (existingUserByUsername != null)
            {
                return Conflict("Username is already taken.");
            }

            // Check if email already exists
            var existingUserByEmail = await _context.users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUserByEmail != null)
            {
                return Conflict("Email is already taken.");
            }

            // Create the new user
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Password = dto.Password,  // Ideally, you should hash the password here
            };

            _context.users.Add(user);
            await _context.SaveChangesAsync();

            return Ok( user.UserId );
        }





        [HttpPost("login")]
        public async Task<IActionResult> LogInApi( LoginUserDTO dto)
        {
            if ( string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Invalid login data.");
            }

            // Check if the email exists
            var existingUser = await _context.users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Here you would verify the password (e.g., by hashing it and comparing with the stored hashed password)
            if (existingUser.Password != dto.Password)  // Replace with proper password hashing comparison
            {
                return Unauthorized("Invalid email or password.");
            }

            // Assuming UserId is a property of the User entity
            return Ok( existingUser.UserId);
        }
    }
}
