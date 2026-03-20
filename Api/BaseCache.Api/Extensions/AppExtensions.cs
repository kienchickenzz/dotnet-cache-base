namespace BaseCache.Api.Extensions;


public static class AppExtensions
{
    public static void UseSwaggerExtension(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var version in app.DescribeApiVersions().Select(version => version.GroupName))
                options.SwaggerEndpoint($"/swagger/{version}/swagger.json", version);
        });
    }
}
