using Ollama.API;
using Platform.Lib.API.Extensions;
using Platform.Lib.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, App>(PermissionConstants.GetPermissions);

builder.AddGrafanaOTEL("Ollama-API");

var app = builder.Build();

app.UsePlatform<Program>();

app.Run();
