using Platform.Lib.API.Extensions;
using Platform.Lib.Constants;
using Reporting.API;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, App>(PermissionConstants.GetPermissions);

builder.AddGrafanaOTEL("Reporting.API");

var app = builder.Build();

//Seed db on startup 
using (var scope = app.Services.CreateScope())
{
    //await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

app.UsePlatform<Program>();

app.Run();
