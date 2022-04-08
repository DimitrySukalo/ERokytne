namespace ERokytne.Api.Infrastructure.Extensions;

public static class SwaggerExtension
{
    public static void AddSwagger(this WebApplication webApplication, string pathBase, IConfiguration configuration)
    {
        webApplication.UseSwaggerUI(setup =>
        {
            setup.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", $"{configuration.GetValue<string>("General:ServiceName")} v1");

            if (!webApplication.Environment.IsDevelopment())
            {
                return;
            }
            
            setup.OAuthClientId("ui.client");
            setup.OAuthClientSecret("secret");
        });
    }
}