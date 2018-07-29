using System;
using MySql.Data.MySqlClient;

namespace Database
{
    public class MySql : IDisposable
    {
        private struct Database
        {
            public const string Server = "35.189.17.176";
            public const string Port = "3306";
            public const string Name = "price_sets";
            public const string User = "root";
            public const string Password = "root";
        }

        private readonly string _connstring = $"Server={Database.Server}; port={Database.Port}; database={Database.Name}; user={Database.User}; password={Database.Password}; SslMode=none";

        private static MySql _instance;

        public MySqlConnection Connection { get; private set; }

        public static MySql Instance()
        {
            return _instance ?? (_instance = new MySql());
        }

        public bool IsConnected()
        {
            if (Connection == null)
            {
                Connection = new MySqlConnection(_connstring);
                Connection.Open();
            }
            return true;
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}
