using API.Data;
using API.Extensions;
using API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.AddApplicationServices();
builder.AddAuthentiocanServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(opt =>
{
    opt.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins(builder.Configuration["JWT:ClientUrl"]);
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


await InitializeContextAsync();
app.Run();


async Task InitializeContextAsync()
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = scope.ServiceProvider.GetService<Context>();
        var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();

        await ContextInitializer.InitializeAsync(context, userManager);
    }
    catch(Exception ex)
    {
        var logger = services.GetService<ILogger<Program>>();
        logger.LogError(ex, "Error occured during application migration");
    }
}