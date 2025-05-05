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
             
 
            var existingUserByUsername = await _context.users
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName);
            if (existingUserByUsername != null)
            {
                return Conflict("Username is already taken.");
            }

            
            var existingUserByEmail = await _context.users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUserByEmail != null)
            {
                return Conflict("Email is already taken.");
            }

            
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Password = dto.Password,  // Ideally, you should hash the password here
            };

            _context.users.Add(user);
            await _context.SaveChangesAsync();

            var token = JwtHelper.GenerateToken(user.UserId.ToString());

            return Ok(new { token });
        }





        [HttpPost("login")]
        public async Task<IActionResult> LogInApi( LoginUserDTO dto)
        {
             
            // Check if the email exists
            var existingUser = await _context.users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser == null)
            {
                return Unauthorized("Invalid email or password.");
            }

           
            if (existingUser.Password != dto.Password)   
            {
                return Unauthorized("Invalid email or password.");
            }
            var token = JwtHelper.GenerateToken(existingUser.UserId.ToString());

            return Ok(new { token });

        }
    }
}
