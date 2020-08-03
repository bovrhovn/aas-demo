using System;

using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace aas.web.api.classic.Models
{
    public enum AuthScheme
    {
        NONE,
        BEARER
    }
    
    internal class AuthData
    {
        public AuthData() => Scheme = AuthScheme.BEARER;

        public async Task<AuthData> LoginAsync()
        {
            var authContext = new AuthenticationContext(AASSettings.Authority);
            var clientCred = new ClientCredential(AASSettings.ClientId, AASSettings.Secret);
            var result = await authContext.AcquireTokenAsync(AASSettings.ResourceId, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            var authData = new AuthData { PasswordOrToken = result.AccessToken};
            return authData;
        }
        
        public AuthScheme Scheme { get; set; }
        public string PasswordOrToken { get; set; }
        public DateTime ValidTo { get; set; }
    }
}