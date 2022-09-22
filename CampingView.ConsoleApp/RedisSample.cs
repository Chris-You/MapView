using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace CampingView.ConsoleApp
{
    public class RedisSample
    {
        private ConnectionMultiplexer _conntction;
        public IDatabase redisDatabase;
        public IServer redisServer;

        public RedisSample(string host, int port, string pass)
        {
            this._conntction = ConnectionMultiplexer.Connect(host + ":" + port + ",password=" + pass);
            if (_conntction.IsConnected)
            {
                this.redisDatabase = this._conntction.GetDatabase();
                this.redisServer = this._conntction.GetServer(host + ":" + port);
            }
        }
    }
}
