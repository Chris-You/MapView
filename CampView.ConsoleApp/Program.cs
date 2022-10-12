using System;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CampingView.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===== Sample World! =====");


            var dbClient = new MongoClient("mongodb://146.56.155.124:27017");
            var dbList = dbClient.ListDatabases().ToList();

            Console.WriteLine("The list of databases are:");

            foreach (var item in dbList)
            {
                Console.WriteLine(item);
            }


            
            IMongoDatabase db = dbClient.GetDatabase("testdb");
            
            var command = new BsonDocument { { "dbstats", 1 } };
            var result = db.RunCommand<BsonDocument>(command);
            Console.WriteLine(result.ToJson());


            var cars = db.GetCollection<BsonDocument>("cars");

            /*
            var filter = Builders<BsonDocument>.Filter.Eq("price", 29000);
            var doc = cars.Find(filter).FirstOrDefault();
            Console.WriteLine(doc.ToString());
            */


            /*
            var doc2 = new BsonDocument
            {
                {"name", "BMW"},
                {"price", 34621}
            };

            cars.InsertOne(doc2);
            */

            /*
            var filter = Builders<BsonDocument>.Filter.Eq("name", "BMW");
            cars.DeleteOne(filter);
            */

            var filter = Builders<BsonDocument>.Filter.Eq("name", "Audi");
            var update = Builders<BsonDocument>.Update.Set("price", 52000);

            cars.UpdateOne(filter, update);


            var documents = cars.Find(new BsonDocument()).ToList();
            foreach (BsonDocument d in documents)
            {
                Console.WriteLine(d.ToString());
            }




        }
    }
}
