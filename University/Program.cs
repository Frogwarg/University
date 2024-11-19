using University.Models;
using Microsoft.EntityFrameworkCore;
using University;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Логирование в консоль
builder.Logging.SetMinimumLevel(LogLevel.Information);


builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 2,  // Уменьшаем количество попыток
            maxRetryDelay: TimeSpan.FromSeconds(3),  // Уменьшаем задержку
            errorCodesToAdd: null);

        // Включаем мультиплексирование для оптимизации подключений
        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    })
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationContext>()
        .AddDefaultTokenProviders();
builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("AppLogger");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationContext>();
    DataInitializer.Initialize(dbContext);
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    await CreateRolesAndAdminAsync(roleManager, userManager, services);
    //dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}",
    defaults: new { controller = "Home", action = "Index" }
);

app.MapRazorPages();

app.Run();

async Task CreateRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IServiceProvider serviceProvider)
{
    try
    {
        //// Проверяем существование ролей перед созданием
        //if (!await roleManager.RoleExistsAsync("Admin"))
        //{
        //    await roleManager.CreateAsync(new IdentityRole("Admin"));
        //}

        //// Проверяем существование администратора перед созданием
        //var adminEmail = "admin@example.com"; // Ваш email админа
        //var adminUser = await userManager.FindByEmailAsync(adminEmail);

        //if (adminUser == null)
        //{
        //    var admin = new ApplicationUser
        //    {
        //        UserName = adminEmail,
        //        Email = adminEmail,
        //        EmailConfirmed = true
        //    };

        //    var result = await userManager.CreateAsync(admin, "AdminPassword123!"); // Ваш пароль
        //    if (result.Succeeded)
        //    {
        //        await userManager.AddToRoleAsync(admin, "Admin");
        //    }
        //}
        logger.LogInformation("Начало создания ролей и администратора.");

        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                logger.LogInformation($"Создание роли: {roleName}");
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            else
            {
                logger.LogInformation($"Роль {roleName} уже существует.");
            }
        }

        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            logger.LogInformation("Создание учетной записи администратора.");
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };
            var createAdminResult = await userManager.CreateAsync(admin, "AdminPassword123!");
            if (createAdminResult.Succeeded)
            {
                logger.LogInformation("Администратор успешно создан.");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                logger.LogError("Не удалось создать администратора: " + string.Join(", ", createAdminResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Администратор уже существует.");
        }
    }
    catch (Exception ex)
    {
        // Логируем ошибку, но не останавливаем запуск приложения
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при инициализации ролей и администратора");
    }
}