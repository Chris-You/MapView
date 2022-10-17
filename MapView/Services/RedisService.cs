using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace MapView.Services
{
    public class RedisService
    {
        public IDatabase redisDatabase;
        public IServer redisServer;

        private ConnectionMultiplexer _conntction;
        
        public RedisService(string host, string port, string pass) : this(host, port, pass, "0")
        {
        }

        public RedisService(string host, string port, string pass, string db)
        {
            this._conntction = ConnectionMultiplexer.Connect(host + ":" + port + ",password=" + pass + ",DefaultDatabase=" + db);
            if (_conntction.IsConnected)
            {
                this.redisDatabase = this._conntction.GetDatabase();
                this.redisServer = this._conntction.GetServer(host + ":" + port);
            }
        }

    }
}
