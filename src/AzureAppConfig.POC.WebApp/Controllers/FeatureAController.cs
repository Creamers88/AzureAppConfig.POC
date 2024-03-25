using AzureAppConfig.POC.WebApp.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace AzureAppConfig.POC.WebApp.Controllers
{
    [FeatureGate(FeatureFlags.FeatureA)]
    public class FeatureAController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
