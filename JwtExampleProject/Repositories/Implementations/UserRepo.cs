using BCrypt.Net;
using JwtExampleProject.ApiResponses;
using JwtExampleProject.Data;
using JwtExampleProject.DTOs;
using JwtExampleProject.Entities;
using JwtExampleProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtExampleProject.Repositories.Implementations
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRepo(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginUserResponse> LoginUserAsync(LoginUserDto loginUserDto)
        {
            var userFromDb = await _context.ApplicationUsers.FirstOrDefaultAsync(au => au.Email == loginUserDto.Email);

            if (userFromDb == null)
            {
                return new LoginUserResponse() { Flag = false, Message = "Sorry user not found bro!!!" };
            }

            bool isPasswordVerified = BCrypt.Net.BCrypt.Verify(loginUserDto.Password, userFromDb.Password);

            if (isPasswordVerified)
            {
                return new LoginUserResponse() { Flag = true, Message = "You have been logged in Successfully!!!", Token = GenerateJwtToken(userFromDb) };
            }
            else
            {
                return new LoginUserResponse() { Flag = false, Message = "Invalid Credentials!!!" };
            }
        }

        private string GenerateJwtToken(ApplicationUser userFromDb)
        {
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new Claim[] 
            {
                new Claim(ClaimTypes.NameIdentifier, userFromDb.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromDb.Name),
                new Claim(ClaimTypes.Email, userFromDb.Email),
            };

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims : claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signingCredentials
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return jwtToken;
        }   

        public async Task<RegisterUserResponse> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var userFromDb = await _context.ApplicationUsers.SingleOrDefaultAsync( au => au.Email == registerUserDto.Email);

            if (userFromDb != null)
            {
                return new RegisterUserResponse() { Flag = false, Message = "User already exist bro!!!" };
            }
            else
            {
                ApplicationUser applicationUser = new()
                {
                    Name = registerUserDto.Name,
                    Email = registerUserDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password)
                };
                await _context.ApplicationUsers.AddAsync(applicationUser);
                await  _context.SaveChangesAsync();
            }
            return new RegisterUserResponse() { Flag = true, Message = "You have been registered successfully" };
        }
    }
}
