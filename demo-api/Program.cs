
using Application.IService;
using Application.Service;
using demo_api.Controllers;
using demo_api.Exceptions;
using demo_api.Middlewares;
using Domain.IRepository;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
namespace demo_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddProblemDetails();
            
            builder.Services.AddExceptionHandler<CustomExceptionHandler>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowAll",
                policy =>
                {
                 policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                 .AllowCredentials();
                });
            });


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero, 

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AccessKey"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async context => 
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            throw new CustomException("Token expired",401);

                        }
                    }
                };

             });

            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IRoleService, RoleService>();


            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();

            builder.Services.AddScoped<IBlackListRepository, BlackListRepository>();
            builder.Services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(Assembly.Load("Application"));
            });

            var app = builder.Build();
       
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAll");

            app.UseHttpsRedirection();
            app.UseExceptionHandler();

            app.UseAuthentication();

            app.UseMiddleware<CheckBlackListMiddleware>();

            app.UseMiddleware<GetRolesMiddleware>();
            app.UseAuthorization();
            app.UseMiddleware<ResponseWrapperApi>();

            app.MapControllers();

            app.Run();
        }
    }
}
