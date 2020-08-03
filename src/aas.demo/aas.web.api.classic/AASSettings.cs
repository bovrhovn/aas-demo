using System.Configuration;

namespace aas.web.api.classic
{
    /// <summary>
    /// settings from configuration file for Azure Analysis Server and demo
    /// </summary>
    public static class AASSettings
    {
        public static string DataSource = ConfigurationManager.AppSettings["AAS-DataSource"];
        public static string DataBase = ConfigurationManager.AppSettings["AAS-DataBase"];
        public static string Authority = ConfigurationManager.AppSettings["Azure-Authority"];
        public static string ResourceId = ConfigurationManager.AppSettings["Azure-ResourceId"];
        public static string ClientId = ConfigurationManager.AppSettings["Azure-ClientId"];
        public static string Secret = ConfigurationManager.AppSettings["Azure-Secret"];
        public static string TenantId = ConfigurationManager.AppSettings["Azure-TenantId"];
        public static string SubcriptionId = ConfigurationManager.AppSettings["Azure-SubscriptionId"];
    }
}