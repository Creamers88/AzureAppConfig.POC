namespace AzureAppConfig.POC.WebApp.Models
{
    public class ConfigSettings
    {
        public MainHeaderSettings MainHeader { get; set; }

        public ForegroundSettings Foreground { get; set; }

        public SubHeaderSettings SubHeader { get; set; }
    }

    public class MainHeaderSettings
    {
        public string Colour { get; set; }

        public string Text { get; set; }
    }

    public class ForegroundSettings
    {
        public string Colour { get; set; }
    }

    public class SubHeaderSettings
    {
        public string HtmlMarkup { get; set; }
    }
}
