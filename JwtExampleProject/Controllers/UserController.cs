using JwtExampleProject.DTOs;
using JwtExampleProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JwtExampleProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;

        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegisterUserDto>> Register(RegisterUserDto registerUserDto)
        {
            var result = await _userRepo.RegisterUserAsync(registerUserDto);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginUserDto>> Login(LoginUserDto loginUserDto)
        {
            var result = await _userRepo.LoginUserAsync(loginUserDto);
            return Ok(result);
        }
    }
}
