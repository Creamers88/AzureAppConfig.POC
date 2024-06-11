using Microsoft.FeatureManagement.FeatureFilters;

namespace AzureAppConfig.POC.WebApp.FeatureManagement.Filters
{
    public class CurrentUserTargetingContextAccessor
        (IHttpContextAccessor httpContextAccessor) : ITargetingContextAccessor
    {
        private const string TargetingContextLookup = "CurrentUserTargetingContextAccessor.TargetingContext";

        public ValueTask<TargetingContext> GetContextAsync()
        {
            HttpContext httpContext = httpContextAccessor.HttpContext;
            if (httpContext.Items.TryGetValue(TargetingContextLookup, out object value))
            {
                return new ValueTask<TargetingContext>((TargetingContext)value);
            }
            List<string> groups = new List<string>();
            if (httpContext.User.Identity.Name != null)
            {
                groups.Add(httpContext.User.Identity.Name.Split("@", StringSplitOptions.None)[1]);
            }

            foreach (var group in httpContext.User.Claims.Where(x => x.Type == "groups"))
            {
                groups.Add(group.Value);
            }

            TargetingContext targetingContext = new TargetingContext
            {
                UserId = httpContext.User.Identity.Name,
                Groups = groups
            };
            httpContext.Items[TargetingContextLookup] = targetingContext;
            return new ValueTask<TargetingContext>(targetingContext);
        }
    }
}
