using Email_Worker2.DataRepository.Repositories;
using Email_Worker2.DataRepository;
using Email_Worker2.DTOs.ConfigDTO;
using Email_Worker2.EmailWorker;
using Email_Worker2.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));


        services.Configure<EmailSettings>(hostContext.Configuration.GetSection("EmailSettings"));
        services.Configure<WorkerSettings>(hostContext.Configuration.GetSection("WorkerSettings"));


        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddScoped<IUsersService, UserService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();

        services.AddSingleton<EmailWorker>();

    })
    .UseWindowsService();



var host = builder.Build();

//For  initilizing and seeding the database
using (var scope = host.Services.CreateScope())
{
    try
    {
        await SeedData.InitializeAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "The database has failed to intialize");
        throw;
    }
}

Task.Run(async () =>
{
    using var scope = host.Services.CreateScope();
    var emailWorker = scope.ServiceProvider.GetRequiredService<EmailWorker>();
    await emailWorker.RunEmailWorkerAsync();
});

await host.RunAsync();

