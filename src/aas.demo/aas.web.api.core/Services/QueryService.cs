using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using aas.web.api.Interfaces;
using aas.web.api.Models;
using aas.web.api.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace aas.web.api.Services
{
    public class QueryService : IQueryService
    {
        private readonly ILogger<QueryService> logger;
        private readonly AASSettings settings;
        private readonly AzureSettings azureSettings;

        public QueryService(ILogger<QueryService> logger, IOptions<AASSettings> settingsValue,
            IOptions<AzureSettings> azureSettingsValue)
        {
            this.logger = logger;
            azureSettings = azureSettingsValue.Value;
            settings = settingsValue.Value;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        public async Task<AuthData> AuthenticateAsync()
        {
            var authContext = new AuthenticationContext(azureSettings.Authority);
            var clientCred = new ClientCredential(azureSettings.ClientId, azureSettings.Secret);
            var result = await authContext.AcquireTokenAsync(azureSettings.ResourceId, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            var authData = new AuthData {Scheme = AuthScheme.BEARER, PasswordOrToken = result.AccessToken};
            return authData;
        }

        public async Task<byte[]> QueryAsync(AuthData authData, Dictionary<string, string> queryString)
        {
            if (!queryString.ContainsKey("query"))
                throw new Exception("get request must include 1 'query' query string parameter",
                    new ArgumentException("query"));

            var cancel = new CancellationToken();

            logger.LogInformation("Begin request at " + DateTime.Now);

            var server = settings.DataSource;
            var database = settings.Database;

            string constr = string.Empty;
            switch (authData.Scheme)
            {
                case AuthScheme.BASIC:
                    constr =
                        $"Data Source={server};User Id={authData.UPN};Password={authData.PasswordOrToken};Catalog={database};Persist Security Info=True; Impersonation Level=Impersonate";
                    break;
                case AuthScheme.BEARER:
                    constr =
                        $"Data Source={server};Password={authData.PasswordOrToken};Catalog={database};Persist Security Info=True; Impersonation Level=Impersonate";
                    break;
                case AuthScheme.NONE:
                    break;
                default:
                    throw new InvalidOperationException($"unexpected state authData.Scheme={authData.Scheme}");
            }

            //get gzip setting - by default false, but we can change that with setting header file
            bool gzip = !queryString.ContainsKey("gzip") || bool.Parse(queryString["gzip"]);
            //if (req.Headers.AcceptEncoding.Any(h => h.Value == "gzip" || h.Value == "*"))
            //{
            //    gzip = true;
            //}

            //IF using POST method
            // var req = new HttpRequestMessage();
            // req.Headers.Add("Authorization", $"bearer {authData.PasswordOrToken}");
            //var query = req.Method == HttpMethod.Get ? queryString["query"] : await req.Content.ReadAsStringAsync();
            var query = queryString["query"];

            var con = ConnectionPool.Instance.GetConnection(constr, authData);

            var cmd = con.Connection.CreateCommand();
            cmd.CommandText = query;

            object queryResults;

            try
            {
                cmd.CommandTimeout = 2 * 60;
                cancel.Register(() =>
                {
                    cmd.Cancel();
                    con.Connection.Dispose();
                    logger.LogInformation("QueryAsync Execution Canceled!");
                });
                queryResults = cmd.Execute();
            }
            catch (Exception ex)
            {
                throw new Exception($"Server error - command coundnt execute! {ex.Message}");
            }
            
            //var indent = true;

            var content = new PushStreamContent(async (responseStream, content, transportContext) =>
            {
                try
                {
                    Stream encodingStream = responseStream;
                    if (gzip)
                        encodingStream = new GZipStream(responseStream, CompressionMode.Compress, false);

                    await using (responseStream)
                    await using (encodingStream)
                    {
                        await ResultWriter.WriteResultsToStream(queryResults, encodingStream, cancel);

                        ConnectionPool.Instance.ReturnConnection(con);
                        await encodingStream.FlushAsync(cancel);
                        await responseStream.FlushAsync(cancel);
                    }
                }
                catch (Exception ex)
                {
                    con.Connection.Dispose(); //do not return to pool
                    throw new Exception($"Connection pool error occured - {ex.Message}");
                }
            }, "application/json");
            
            var stream = await content.ReadAsStreamAsync();
            var ms = new MemoryStream();
            
            if (gzip)
            {
                await using var gs = new GZipStream(stream, CompressionMode.Decompress);
                await gs.CopyToAsync(ms, cancel);
            }
            else
            {
                await stream.CopyToAsync(ms, cancel);
            }
            
            ms.Position = 0;
            var bytes = ms.ToArray();

            return bytes;
        }
    }
}