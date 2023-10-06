using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using WebApplication1.Helpers;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.UtilityServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class forgetpwrdController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public forgetpwrdController(IConfiguration configuration, IEmailService emailService)
        {
          _configuration= configuration;
          _emailService= emailService;
        }

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            var user = await _authContext.Users.FirstOrDefaultAsync(a => a.Email == email);
            if (user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "email Doesn't Exit"
                });
            }
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetPsswordExpiry = DateTime.Now.AddMinutes(15);
            string from= _configuration["emailSettings:From"];
            var emailModel = new EmailModel(email, "ResetPassword!!", EmailBody.EmailStringBody(email, emailToken));
            _emailService.SendEmail(user, emailModel);
            _authContext.Entry(user).State = EntityState.Modified;
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent!"
            });
             
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult>ResetPassword(ResetPwrdDto ResetPwrdDto)
        {
            var newToken = ResetPwrdDto.EmailToken.Replace(" ", "+");
            var user = await _authContext.Users.AsNoTracking().FirstOrDefaultAsync(a => a.Email == ResetPwrdDto.Email );
            if(user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User Doesn't Exist"
                });
            }
            var tokenCode = user.ResetPasswordToken;
            DateTime emailTokenExpiry = user.ResetPsswordExpiry;
            if(tokenCode != ResetPwrdDto.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode =400,
                    Message = "Invalid Reset link"
                });
            }
            user.Password + PasswordHasher.HashPassword(ResetPwrdDto.NewPassword);
            _authContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Password Reset Successfuly"
            });
        }
    }

    
}
