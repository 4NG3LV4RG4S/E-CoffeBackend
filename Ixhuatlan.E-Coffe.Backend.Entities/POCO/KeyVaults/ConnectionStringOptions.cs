using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Ixhuatlan.E_Coffe.Backend.Entities.POCO.KeyVaults
{
    public class ConnectionStringsOptions
    {
        private string DbConnectionString;
        private readonly object LockObject = new();
        private bool ISConnectionStringLoaded;

        public const string SectionKey = "Secret";
        public string UrlKeyVault { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Secret { get; set; }

        public string ConnectionString
        {
            get
            {
                if (ISConnectionStringLoaded == false)
                {
                    lock (LockObject)
                    {
                        if (ISConnectionStringLoaded == false)
                        {
                            SecretClient secretClient = new SecretClient(new Uri(UrlKeyVault), new ClientSecretCredential(tenantId: TenantId, clientId: ClientId, clientSecret: ClientSecret));
                            DbConnectionString = secretClient.GetSecret(Secret).Value.Value;
                            ISConnectionStringLoaded = true;
                        }
                    }
                }
                return DbConnectionString;
            }
        }
    }
}
