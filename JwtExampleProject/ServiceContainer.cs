using JwtExampleProject.Data;
using JwtExampleProject.Repositories.Implementations;
using JwtExampleProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JwtExampleProject
{
    public static class ServiceContainer
    {
        public static void RegisterDependencies(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                string connectionString = configuration["ConnectionStrings:DbConnection"];
                option.UseSqlServer(connectionString);
            });

            //This is required for enabling any type of authentication (like JWT, cookies, etc.)
            services.AddAuthentication( option =>
            {
                //means the app will use the JWT Bearer token method to verify
                //the user's identity by default.

                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //This setup ensures that the JWT tokens received from clients are validated correctly
            //before granting access to protected resources.
            //It defines the rules and parameters for validating the tokens.

            .AddJwtBearer( option =>
            {
                //This is where you specify the rules for validating the incoming JWT tokens.
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true, 
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidIssuer = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });

            services.AddScoped<IUserRepo, UserRepo>();
        }
    }
}
