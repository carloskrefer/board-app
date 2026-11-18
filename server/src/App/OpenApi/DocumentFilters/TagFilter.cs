using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.OpenApi.DocumentFilters;

public class TagFilter : IDocumentFilter {
    public void Apply(OpenApiDocument doc, DocumentFilterContext ctx) {
        doc.Tags = [
            OpenApiTagFactory.CreateAuthTag()
        ];
    }
}
