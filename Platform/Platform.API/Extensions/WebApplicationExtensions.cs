using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Platform.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UsePlatform(
            this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            // Enable Swagger
            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                app.MapOpenApi();
                app.UseSwagger();
            }

            app.UseAuthorization();
            app.UseExceptionHandler();
            app.UseRequestLocalization();
            app.MapControllers();


            return app;
        }
    }
}
