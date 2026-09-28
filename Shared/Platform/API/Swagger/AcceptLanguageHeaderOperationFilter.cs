using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Platform.Lib.API.Swagger
{
    public class AcceptLanguageHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Preferred language for the response (e.g., en-US, ar, fr)",
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Default = "en"
                }
            });
        }
    }
}