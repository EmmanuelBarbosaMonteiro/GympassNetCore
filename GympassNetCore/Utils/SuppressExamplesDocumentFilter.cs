using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GympassNetCore.Utils
{
    public class SuppressExamplesDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var path in swaggerDoc.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    foreach (var response in operation.Value.Responses)
                    {
                        if (response.Key == "400" || response.Key == "401" || response.Key == "403" || response.Key == "404" || response.Key == "409" || response.Key == "500")
                        {
                            response.Value.Content.Clear();
                        }
                    }
                }
            }
        }
    }
}