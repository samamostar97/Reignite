using DotNetEnv;
using Reignite.API.Extensions;
using Reignite.API.Middleware;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services
    .AddInfrastructure()
    .AddJwtAuthentication()
    .AddSwaggerWithAuth()
    .AddCorsPolicy(builder.Environment)
    .AddControllers();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
