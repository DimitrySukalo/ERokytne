using Hangfire.Dashboard;

namespace ERokytne.Api.Filters;

public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity!.IsAuthenticated;
    }
}