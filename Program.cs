using Microsoft.AspNetCore.Diagnostics;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Application started running.");


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();


    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            
            if (feature.Error is not null)
            {
                logger.Error(feature.Error, "Exception thrown here!");
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(feature.Error.Message);
            }
        });
    });
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}

catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}








