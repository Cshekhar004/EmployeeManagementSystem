using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Helpers;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.Use(async (context, next) =>
{
    bool isLoggedIn =
        !string.IsNullOrEmpty(
            context.Session.GetString("Username"));

    if (!isLoggedIn &&
        context.Request.Cookies.TryGetValue(
            "EMS_RememberMe",
            out string? rememberToken))
    {
        using var scope =
            context.RequestServices.CreateScope();

        var db =
            scope.ServiceProvider
            .GetRequiredService<EmployeeDbContext>();

        string tokenHash =
            RememberMeHelper.HashToken(rememberToken);

        var user =
            await db.Users.FirstOrDefaultAsync(u =>
                u.RememberTokenHash == tokenHash &&
                u.RememberTokenExpiry > DateTime.Now &&
                u.IsActive &&
                !u.IsDeleted);

        if (user != null)
        {
            context.Session.SetString(
                "Username",
                user.Username);

            context.Session.SetString(
                "Role",
                user.Role);

            context.Session.SetInt32(
                "UserId",
                user.UserId);

            context.Session.SetString(
                "MustChangePassword",
                user.MustChangePassword.ToString());
        }
        else
        {
            context.Response.Cookies.Delete(
                "EMS_RememberMe");
        }
    }

    await next();
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();