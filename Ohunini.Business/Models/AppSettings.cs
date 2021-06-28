namespace Ohunini.Business.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateAudience { get; set; }
        public string Scheme { get; set; }
    }

    /// <summary>
    /// Swagger configuration
    /// </summary>
    public class SwaggerConfiguration
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
    }
}