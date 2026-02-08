using DotNetEnv;
using Reignite.API.Extensions;
using Reignite.API.Middleware;
using Reignite.Infrastructure.Data;

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

// Database: migrate + seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();
    var webRootPath = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
    await DatabaseSeeder.SeedAsync(context, webRootPath);
}

Console.WriteLine();
Console.WriteLine("======================================================");
Console.WriteLine("  REIGNITE - Test Accounts");
Console.WriteLine("  Admin:  admin@reignite.ba / test");
Console.WriteLine("  User:   test@reignite.ba  / test");
Console.WriteLine("------------------------------------------------------");
Console.WriteLine("  Backend:  dotnet run (from Reignite.API/)");
Console.WriteLine("  Frontend: npm install && npm start (from client/)");
Console.WriteLine("  Slike se preuzimaju pri prvom pokretanju (internet).");
Console.WriteLine("======================================================");
Console.WriteLine();

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

await app.RunAsync();
