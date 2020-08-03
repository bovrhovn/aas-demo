using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using aas.web.api.classic.Models;

namespace aas.web.api.classic.Controllers
{
    [Route("query")]
    public class QueryController : ApiController
    {
        static QueryController() => ServicePointManager.DefaultConnectionLimit = int.MaxValue;

        [HttpGet]
        [Route("data/{query}")]
        public async Task<string> Get(string query)
        {
            var server = AASSettings.DataBase;
            var database = AASSettings.DataSource;

            if (string.IsNullOrEmpty(server))
                throw new InvalidOperationException("Required AppSettings Server/database is missing.");
            if (string.IsNullOrEmpty(database))
                throw new InvalidOperationException("Required AppSettings DataSource is missing.");

            var authData = await new AuthData().LoginAsync();
            var constr = authData.Scheme == AuthScheme.BEARER
                ? $"Data Source={server};Password={authData.PasswordOrToken};Catalog={database};Persist Security Info=True; Impersonation Level=Impersonate"
                : throw new InvalidOperationException($"unexpected state authData.Scheme={authData.Scheme}");

            if (string.IsNullOrEmpty(query)) return "get request must include 1 'query' query string parameter";
            
            var con = ConnectionPool.Instance.GetConnection(constr, authData);

            var cmd = con.Connection.CreateCommand();
            cmd.CommandText = query;

            object queryResults;

            var cancel = new CancellationToken();
            
            try
            {
                cmd.CommandTimeout = 2 * 60;
                cancel.Register(() =>
                {
                    cmd.Cancel();
                    con.Connection.Dispose();
                });
                queryResults = cmd.Execute();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
            var result = new PushStreamContent(async (responseStream, content, transportContext) =>
            {
                try
                {
                    var encodingStream = responseStream;

                    using (responseStream)
                    using (encodingStream)
                    {
                        await ResultWriter.WriteResultsToStream(queryResults, encodingStream, cancel);

                        ConnectionPool.Instance.ReturnConnection(con);
                        await encodingStream.FlushAsync(cancel);
                        await responseStream.FlushAsync(cancel);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    con.Connection.Dispose(); //do not return to pool
                    throw;
                } }, "application/json");

            var json = await result.ReadAsStringAsync();

            if (string.IsNullOrEmpty(json))
                json = "No data returned! Check query!";
            
            return json;
        }
    }
}