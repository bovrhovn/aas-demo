using System;
using System.Collections.Concurrent;
using Microsoft.AnalysisServices.AdomdClient;

namespace aas.web.api.classic.Models
{
    internal class ConnectionPoolEntry
    {
        public ConnectionPoolEntry(AdomdConnection con, string connectionString)
        {
            Connection = con;
            ConnectionString = connectionString;
            con.Disposed += (s, a) =>
            {
                IsDisposed = true;
                con = null;
            };
        }
        
        public bool IsDisposed { get; private set; } = false;
        public string ConnectionString { get; private set; }
        public AdomdConnection Connection { get; private set; }
        public DateTime ValidTo { get; set; }

        public void RecordCheckIn()
        {
            IsCheckedOut = false;
            TotalCheckoutTime += DateTime.Now.Subtract(LastCheckedOut);
            LastCheckedIn = DateTime.Now;
        }

        public void RecordCheckOut()
        {
            IsCheckedOut = true;
            LastCheckedOut = DateTime.Now;
        }
        
        public bool IsCheckedOut { get; private set; }
        public int TimesCheckedOut { get; private set; }
        public TimeSpan TotalCheckoutTime { get; private set; }
        public DateTime LastCheckedOut { get; private set; } = DateTime.MinValue;
        public DateTime LastCheckedIn { get; private set; } = DateTime.MinValue;
    }

    internal class ConnectionPool
    {
        public static readonly ConnectionPool Instance = new ConnectionPool();
        readonly ConcurrentDictionary<string, ConcurrentStack<ConnectionPoolEntry>> avalableConnections = new ConcurrentDictionary<string, ConcurrentStack<ConnectionPoolEntry>>();

        public void ReturnConnection(ConnectionPoolEntry entry)
        {
            var key = entry.ConnectionString;
            entry.RecordCheckIn();
            avalableConnections[key].Push(entry);
        }
        public ConnectionPoolEntry GetConnection(string connectionString, AuthData authData)
        {
            var key = connectionString;
            ConnectionPoolEntry rv = null;
            avalableConnections.AddOrUpdate(key, k => new ConcurrentStack<ConnectionPoolEntry>(), (k, c) =>
            {
                while (c.TryPop( out var entry ))
                {
                    if (entry.ValidTo > DateTime.Now.Subtract(TimeSpan.FromMinutes(1)))
                    {
                        entry.Connection.Dispose();
                        continue;
                    }

                    rv = entry;
                    break;
                }
                return c;
            });

            if (rv == null)
            {
                var con = new AdomdConnection(connectionString);
                rv = new ConnectionPoolEntry(con, connectionString);

                var validTo = DateTime.Now.AddHours(1); //default
                if (authData.Scheme ==  AuthScheme.BEARER)
                {
                    var token = TokenHelper.ReadToken(authData.PasswordOrToken);
                    validTo = token.ValidTo.ToLocalTime();
                }
                rv.ValidTo = validTo;
                con.Open();
            }
            rv.RecordCheckOut();
            return rv;
        }
    }
}
