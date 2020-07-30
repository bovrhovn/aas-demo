using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using aas.web.api.Models;

namespace aas.web.api.Interfaces
{
    public interface IQueryService
    {
        Task<AuthData> AuthenticateAsync();
        Task<byte[]> QueryAsync(AuthData authData, Dictionary<string, string> queryString);
    }
}