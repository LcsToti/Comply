using Payments.API.Extensions;
using Payments.API.Middlewares;
using Payments.App;
using Payments.Infra;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddControllers();
    builder.Services.AddSwagger();
    builder.Services.AddAuth(builder.Configuration);
    builder.Services.AddApp();
    builder.Services.AddInfra(builder.Configuration);
    builder.Services.AddFluentValidation();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwaggerDocumentation();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
