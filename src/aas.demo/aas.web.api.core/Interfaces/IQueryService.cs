using System.Collections.Generic;
using System.Threading.Tasks;
using aas.demo.shared;

namespace aas.web.api.Interfaces
{
    public interface IQueryService
    {
        Task<AuthData> AuthenticateAsync();
        Task<byte[]> QueryAsync(AuthData authData, Dictionary<string, string> queryString);
    }
}