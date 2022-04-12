using Microsoft.AspNetCore.Localization;

namespace ERokytne.Api.Infrastructure.Extensions;

public static class AppSettingExtension
{
    public static void SetInfrastructureOptions(this WebApplication webApplication, IConfiguration configuration)
    {
        webApplication.UseForwardedHeaders();
        webApplication.UseRequestLocalization(o =>
        {
            var supportedCultures = new[] {"uk", "en", "ru"};
            o.SetDefaultCulture("uk");
            o.AddSupportedCultures(supportedCultures);
            o.AddSupportedUICultures(supportedCultures);
            o.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider()
            };
        });   
        
            
        var pathBase = configuration.GetValue<string>("ASPNETCORE_APPL_PATH");
        if (!string.IsNullOrWhiteSpace(pathBase))
        {
            webApplication.UsePathBase(pathBase);
        }
            
        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseDeveloperExceptionPage();
            webApplication.UseCors("TestCorsPolicy");
        }
            
        webApplication.UseCors("DefaultCorsPolicy");
    }
}