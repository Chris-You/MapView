using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace CampingView.ConsoleApp
{
    public class RedisSample
    {

        private ConnectionMultiplexer _conntction;
        private IDatabase _database;

        public RedisSample()
        {

        }

        public bool Connect(string host, int port, string pass)
        {
            try
            {
                this._conntction = ConnectionMultiplexer.Connect(host + ":" + port + ",password="+ pass);
                if(_conntction.IsConnected)
                {
                    this._database = this._conntction.GetDatabase();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public string GetStr(string key)
        {
            return this._database.StringGet(key);
        }

        public bool SetStr(string key, string val)
        {
            return this._database.StringSet(key, val);
        }

        public string GetHash(string key)
        {
            return this._database.StringGet(key);
        }

        public void SetHash(string key, HashEntry[] hash)
        {
            this._database.HashSet(key, hash);
        }

        public string GetHash(string key, string field)
        {
            var val = this._database.HashGet(key, field);

            return val.ToString();
        }
    }
}
