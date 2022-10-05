using System;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace CampingView.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //RedisTest();
            ChargerCode
        }



        public static void Reflec()
        {



        }

        public static void RedisTest()
        {
            RedisSample redis = new RedisSample("132.226.224.188", 6379, "redis");


            Console.WriteLine(redis.redisDatabase.StringSet("key1", "val1"));
            Console.WriteLine(redis.redisDatabase.StringGet("key1"));

            Console.WriteLine(redis.redisDatabase.StringSet("key2", "val__2"));
            Console.WriteLine(redis.redisDatabase.StringGet("key2"));

            Console.WriteLine("=====");


            HashEntry[] hash =
            {
                new HashEntry("name", "name1"),
                new HashEntry("email", "abc@naver.com")
            };

            redis.redisDatabase.HashSet("h-key", hash);
            Console.WriteLine(redis.redisDatabase.HashGet("h-key", "name") + "====" + redis.redisDatabase.HashGet("h-key", "email"));
            Console.WriteLine(redis.redisDatabase.HashGet("h-key2", "name") + "====" + redis.redisDatabase.HashGet("h-key2", "email"));

            Console.WriteLine("===== set add");


            redis.redisDatabase.SetAdd("sadd", "comment1");
            redis.redisDatabase.SetAdd("sadd", "comment2");
            Console.WriteLine(redis.redisDatabase.SetMembers("sadd").ToString());
            foreach (var i in redis.redisDatabase.SetMembers("sad"))
            {
                Console.WriteLine(i.ToString());
            }



            foreach (var i in redis.redisServer.Keys(0, "*"))
            {
                Console.WriteLine(i.ToString() + ">" + redis.redisDatabase.KeyType(i.ToString()));
            }

            Console.WriteLine("===== sorted Set");
            var score = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            redis.redisDatabase.SortedSetAdd("sorted Set", "name1", score + 2);
            redis.redisDatabase.SortedSetAdd("sorted Set", "name2", score + 1);
            redis.redisDatabase.SortedSetAdd("sorted Set", "name3", score + 3);

            var mem = redis.redisDatabase.SortedSetScan("sorted Set");
            Console.WriteLine(string.Join(",\n", mem));

            //redis.redisDatabase.SortedSetIncrement("sorted Set", "name1", 200);
            Console.WriteLine("===========sorted Set 2");
            Console.WriteLine(string.Join(",\n", redis.redisDatabase.SortedSetScan("sorted Set")));

            /*
            Console.WriteLine("=========== sorted Set 3");
            var set3 = redis.redisDatabase.SortedSetRangeByRank("camp:search:naver_Ul6VeE6CrILcJ8Na5HJfEtefORqfkfyMZxwmTcjde7U", 0, -1, order: Order.Descending);
            foreach(var s in set3)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("=========== sorted Set 4");
            var set4 = redis.redisDatabase.SortedSetRangeByScore("camp:search:naver_Ul6VeE6CrILcJ8Na5HJfEtefORqfkfyMZxwmTcjde7U", 0, -1, Exclude.None, order: Order.Descending);
            foreach (var s in set4)
            {
                Console.WriteLine(s);
            }


            Console.WriteLine("===========");
            Console.WriteLine(string.Join(",\n", redis.redisDatabase.SortedSetRangeByScoreWithScores("camp:search:naver_Ul6VeE6CrILcJ8Na5HJfEtefORqfkfyMZxwmTcjde7U")));
            //Console.WriteLine(string.Join(",\n", redis.redisDatabase.SortedSetRangeByScoreWithScores("camp:search:naver_Ul6VeE6CrILcJ8Na5HJfEtefORqfkfyMZxwmTcjde7U")));

            */

            foreach (var k in redis.redisDatabase.SortedSetRangeByRankWithScores("camp:search:naver_Ul6VeE6CrILcJ8Na5HJfEtefORqfkfyMZxwmTcjde7U", 0, -1, order:Order.Descending).Take(5))
            {
                var keyword = k.ToString().Split(":")[0];
                var date = k.ToString().Split(":")[1];
                Console.WriteLine(keyword + ">" + date);
            }



        }
    }
}
