using NotificationService.API.Extensions;
using NotificationService.App.UseCases.MarkNotificationAsRead;
using NotificationService.Infra;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddControllers();
    builder.Services.AddSwagger();
    builder.Services.AddAuth(builder.Configuration);
}

builder.Services.AddSignalR();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(MarkAsReadCommand).Assembly);
    
    cfg.AddOpenBehavior(typeof(NotificationService.App.Behaviors.ValidationBehavior<,>));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();