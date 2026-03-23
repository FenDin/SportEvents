using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var resetDatabaseOnStartup = builder.Configuration.GetValue<bool>("Database:ResetOnStartup");

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SportsCompetitionsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SportEvents.Auth";
        options.Cookie.HttpOnly = true;
        options.LoginPath = "/User/Login";
        options.LogoutPath = "/User/Logout";
        options.AccessDeniedPath = "/User/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var userIdValue = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdValue, out var userId))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return;
                }

                var db = context.HttpContext.RequestServices.GetRequiredService<SportsCompetitionsDbContext>();
                var user = await db.Users
                    .AsNoTracking()
                    .Include(item => item.idContactNavigation)
                    .Include(item => item.idRoleNavigation)
                    .FirstOrDefaultAsync(item => item.id == userId);

                var currentRole = user?.idRoleNavigation?.title;
                var currentEmail = user?.idContactNavigation?.email;
                var claimedRole = context.Principal?.FindFirstValue(ClaimTypes.Role);
                var claimedEmail = context.Principal?.FindFirstValue(ClaimTypes.Email);

                var identityChanged =
                    user?.idContactNavigation is null
                    || !string.Equals(currentRole, claimedRole, StringComparison.Ordinal)
                    || !string.Equals(currentEmail, claimedEmail, StringComparison.OrdinalIgnoreCase);

                if (identityChanged)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AppPolicies.ManageCatalog,
        policy => policy.RequireRole(AppRoles.Administrator, AppRoles.Organizer));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SportsCompetitionsDbContext>();

    if (resetDatabaseOnStartup)
    {
        await db.Database.EnsureDeletedAsync();
        db.ChangeTracker.Clear();
    }

    await DbInitializer.InitializeAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
