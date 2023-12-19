using AzureAppConfig.POC.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureAppConfig.POC.WebApp.Controllers
{
    public class ConfigController : Controller
    {
        private readonly IOptionsSnapshot<ConfigSettings> _configSettings;

        public ConfigController(IOptionsSnapshot<ConfigSettings> configSettings)
        {
            _configSettings = configSettings;
        }

        public IActionResult Index()
        {
            var configSettingModel = _configSettings.Value;

            return View(configSettingModel);
        }
    }
}
