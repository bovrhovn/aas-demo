using System.IdentityModel.Tokens.Jwt;

namespace aas.web.api.classic.Models
{
    class TokenHelper
    {
        internal static JwtSecurityToken ReadToken(string bearerToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(bearerToken);
            return token;
        }
    }
}
