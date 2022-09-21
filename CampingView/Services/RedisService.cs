using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace CampingView.Services
{
    public class RedisService
    {
        private ConnectionMultiplexer _conntction;
        private IDatabase _database;
        private IServer _server;

        public bool Connect(string host, string port, string pass)
        {
            try
            {
                this._conntction = ConnectionMultiplexer.Connect(host + ":" + port + ",password=" + pass);
                if (_conntction.IsConnected)
                {
                    this._database = this._conntction.GetDatabase();
                    this._server = this._conntction.GetServer(host + ":" + port);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public IEnumerable<RedisKey> StringKeys(string pattern)
        {
            return this._server.Keys(pattern : pattern);
        }

        public string StringGet(string key)
        {
            return this._database.StringGet(key);
        }

        public bool StringSet(string key, string val)
        {
            return this._database.StringSet(key, val);
        }

        public bool KeyExists(string key)
        {
            return this._database.KeyExists(key);
        }


        public void HashSet(string key, HashEntry[] hash)
        {
            this._database.HashSet(key, hash);
        }

        public string HashGet(string key, string field)
        {
            var val = this._database.HashGet(key, field);

            return val.ToString();
        }

    }
}
