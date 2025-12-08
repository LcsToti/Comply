using UserService.Api.Extensions;
using UserService.App;
using UserService.Infra;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddAuth(builder.Configuration);
    builder.Services.AddInfra(builder.Configuration);
    builder.Services.AddApp();
    builder.Services.AddSwaggerDocumentation();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
}

var app = builder.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

app.UseAuthentication();
    
app.UseAuthorization();

app.MapControllers();

app.Run();