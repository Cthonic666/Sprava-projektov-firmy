using System.Security.Claims;
using SpravaProjektovFirmy.Data;
using SpravaProjektovFirmy.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SpravaProjektovFirmy.Extensions;
using SpravaProjektovFirmy.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            };
        }
    );

builder.Services.AddAuthorization();

builder.Services.AddSingleton(sp =>
{
    var env =  sp.GetRequiredService<IWebHostEnvironment>();
    return new ProjectRepozitory(Path.Combine(env.ContentRootPath, "projects.xml"));
});

builder.Services.AddSingleton(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new UserRepozitory(Path.Combine(env.ContentRootPath, "users.xml"));
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// User 

app.MapPost("/api/login", async (HttpContext context, UserDto userDto, UserRepozitory userRepo) =>
{
    var user = userRepo.Login(userDto);
    if (user is null) return Results.Unauthorized();

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Name),
    };
    
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignInAsync(new  ClaimsPrincipal(identity));
    return Results.Ok(new { user.Name });
}).AllowAnonymous();

app.MapPost("/api/register", (UserDto UserDto, UserRepozitory userRepo) =>
{
    var user = userRepo.RegisterUser(UserDto);
    if (user is null) return Results.Unauthorized();
    
    return Results.Ok(new { user.Name });
}).AllowAnonymous();

app.MapPost("/api/logout", async (HttpContext context) =>
{
    await context.SignOutAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.MapGet("/api/me", (ClaimsPrincipal user) => new
{
    Name = user.Identity!.Name
}).RequireAuthorization();

// Projects

var projects = app.MapGroup("/api/projects").RequireAuthorization();

projects.MapGet("/", (ProjectRepozitory projectRepo, ClaimsPrincipal user) =>
{
    return projectRepo.GetAllProjects(user.UserId());
});

projects.MapPost("/", (ProjectDto project, ProjectRepozitory projectRepo, ClaimsPrincipal user) =>
{
    return projectRepo.AddProject(project, user.UserId());
});

projects.MapDelete("/{projectId}", (string projectId, ProjectRepozitory projectRepo,  ClaimsPrincipal user) =>
{
    projectRepo.DeleteProject(projectId, user.UserId());
});

projects.MapPut("/{projectId}", (ProjectDto dto, ProjectRepozitory projectRepo, string projectId,  ClaimsPrincipal user) =>
{
    return projectRepo.UpdateProject(dto, user.UserId(), projectId );
});

projects.MapGet("/{projectId}", (ProjectRepozitory projectRepo, string projectId,  ClaimsPrincipal user) =>
{
    return projectRepo.GetProjectById(projectId, user.UserId());
});

app.Run();


