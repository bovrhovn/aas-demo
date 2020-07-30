namespace aas.web.api.Settings
{
    public class AzureSettings
    {
        public string Authority { get; set; } = "https://login.windows.net/common";
        public string ResourceId { get; set; } = "https://*.asazure.windows.net";
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
    }
}