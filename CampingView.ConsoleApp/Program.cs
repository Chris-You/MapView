using System;
using StackExchange.Redis;

namespace CampingView.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            RedisSample redis = new RedisSample();
            redis.Connect("132.226.224.188", 6379, "redis");

            Console.WriteLine(redis.SetStr("key2", "val2"));
            Console.WriteLine(redis.GetStr("key2"));
            Console.WriteLine(redis.SetStr("key2", "val__2"));
            Console.WriteLine(redis.GetStr("key2"));

            Console.WriteLine("=====");


            HashEntry[] hash =
            {
                new HashEntry("name", "name1"),
                new HashEntry("email", "age2")
            };

            redis.SetHash("h-key", hash);
            Console.WriteLine(redis.GetHash("h-key", "name") +"====" + redis.GetHash("h-key", "age"));

            Console.WriteLine("=====");
        }
    }
}
