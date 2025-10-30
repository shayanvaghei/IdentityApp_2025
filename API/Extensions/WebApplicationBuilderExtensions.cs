using API.Data;
using API.DTOs;
using API.Models;
using API.Services;
using API.Services.IServices;
using API.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IServiceUnitOfWork, ServiceUnitOfWork>();
            builder.Services.AddCors();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage).ToArray();

                    var errorResponse = new ApiResponse(400, errors: errors);
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return builder;
        }

        public static WebApplicationBuilder AddAuthentiocanServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = SD.RequiredPasswordLength;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.MaxFailedAccessAttempts = SD.MaxFailedAccessAttempts;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(SD.DefaultLockoutTimeSpanInDays);
            }).AddEntityFrameworkStores<Context>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCookie(opt =>
            {
                opt.Cookie.Name = SD.IdentityAppCookie;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[SD.IdentityAppCookie];
                        return Task.CompletedTask;
                    }
                };
            });

            // Policy creation
            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy(SD.AdminPolicy, policy => policy.RequireRole(SD.AdminRole));
                opt.AddPolicy(SD.ModeratorPolicy, policy => policy.RequireRole(SD.ModeratorRole));
                opt.AddPolicy(SD.UserPolicy, policy => policy.RequireRole(SD.UserRole));

                opt.AddPolicy(SD.AdminOrModeratorPolicy, policy => policy.RequireRole(SD.AdminRole, SD.ModeratorRole));
                opt.AddPolicy(SD.AdminAndModeratorPolicy, policy => policy.RequireRole(SD.AdminRole).RequireRole(SD.ModeratorRole));
                opt.AddPolicy(SD.AllRolePolicy, policy => policy.RequireRole(SD.AdminRole, SD.ModeratorRole, SD.UserRole));

                opt.AddPolicy(SD.AdminEmailPolicy, policy => policy.RequireClaim(SD.Email, SD.SuperAdminEmail));
                opt.AddPolicy(SD.VIPPolicy, policy => policy.RequireAssertion(context => VIPPolicy(context)));
            });

            return builder;
        }

        public static bool VIPPolicy(AuthorizationHandlerContext context)
        {
            if (context.User.IsInRole(SD.UserRole) &&
                context.User.HasClaim(c => c.Type == SD.Email && c.Value.Contains("vip")))
            {
                return true;
            }

            return false;
        }
    }
}
