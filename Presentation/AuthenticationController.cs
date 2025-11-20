using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.Dtos;
using Shared.Dtos.User;
using Shared.Models;

namespace Presentation
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthenticationController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<ActionResult<UserResultDto>> Login(LoginDto loginDto)
        {
            var result = await serviceManager.AuthenticationService.Login(loginDto);
            return Ok(result);
        }
        [HttpPost("register/school")]
        public async Task<ActionResult<UserResultDto>> RegisterSchool(SchoolRegisterDto schoolRegisterDto)
        {
            var result = await serviceManager.AuthenticationService.RegisterSchool(schoolRegisterDto);
            return Ok(result);
        }
        [HttpPost("register/university")]
        public async Task<ActionResult<UserResultDto>> RegisterUniversity(UniversityRegisterDto dto)
        {
            var result = await serviceManager.AuthenticationService.RegisterUniversity(dto);
            return Ok(result);
        }
        [HttpPost("register/vendor")]
        public async Task<ActionResult<UserResultDto>> RegisterVendor(VendorRegisterDto vendorRegisterDto)
        {
            var result = await serviceManager.AuthenticationService.RegisterVendor(vendorRegisterDto);
            return Ok(result);
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            Console.WriteLine($"🔍 Checking email: {email}");
            var result = await serviceManager.AuthenticationService.CheckIfEmailExist(email);
            Console.WriteLine($"✅ Email exists result: {result}");
            return Ok(result);
        }
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await serviceManager.AuthenticationService.SendResetPasswordEmail(model.Email);
            return Ok(new { message = "If your email is registered, you will receive a password reset link" });
        }
        [HttpGet("check-inbox")]
        public ActionResult CheckYourInbox()
        {
            return Ok(new { message = "Please check your email inbox for the password reset link" });
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await serviceManager.AuthenticationService.ResetPassword(model.Email, model.Token, model.Password);
            if (result)
            {
                return Ok(new { message = "Password has been reset successfully" });
            }
            else
            {
                return BadRequest(new { error = "Invalid or expired reset token" });
            }
        }
    }
}
