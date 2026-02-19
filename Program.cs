using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureAgentPortal.Data;
using Serilog;
using SecureAgentPortal.Services;


var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<SecureAgentPortal.Repositories.ITransactionRepository, SecureAgentPortal.Repositories.TransactionRepository>();
builder.Services.AddScoped<SecureAgentPortal.Services.IAuditService, SecureAgentPortal.Services.AuditService>();
builder.Services.AddScoped<SecureAgentPortal.Services.TransactionService>();


var app = builder.Build();

app.UseMiddleware<SecureAgentPortal.Middleware.ExceptionHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//dev endpoint make user admin
// if (app.Environment.IsDevelopment())
// {
//      app.MapPost("/dev/make-admin", async (HttpContext ctx,
//         UserManager<IdentityUser> userManager) =>
//     {
//         if (ctx.User?.Identity?.IsAuthenticated != true) return Results.Unauthorized();

//         var user = await userManager.GetUserAsync(ctx.User);
//         if (user is null) return Results.NotFound();

//         await userManager.AddToRoleAsync(user, "Admin");
//         return Results.Ok("User added to Admin role.");
//     }).RequireAuthorization();
// }

app.UseHttpsRedirection();
app.UseRouting();

app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();



// await SecureAgentPortal.Data.RoleSeeder.SeedAsync(app.Services);
// await SecureAgentPortal.Data.IdentitySeeder.SeedAsync(app.Services, app.Configuration);


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // creates DB + tables if missing
}

await RoleSeeder.SeedAsync(app.Services, app.Configuration);

app.Run();
