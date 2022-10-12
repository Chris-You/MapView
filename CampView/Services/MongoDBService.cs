using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;
using CampView.Models.Charger;

namespace CampView.Services
{
    public class MongoDBService
    {
        private IMongoDatabase db;
        private MongoClient dbClient;

        public MongoDBService(string host, string port, string dbName)
        {
            dbClient = new MongoClient("mongodb://" + host + ":" + port);
            db = dbClient.GetDatabase(dbName);
        }


        public IMongoDatabase GetDatabase(string dbName)
        {
            return dbClient.GetDatabase(dbName);
        }

        public List<ChargerComment> CommentList(string docName)
        {
            var comments = db.GetCollection<ChargerComment>(docName);
            
            var docs = comments.Find(new BsonDocument()).ToList();
            if (docs.Count > 0)
                return docs;

            else
                return null;
        }


        public ChargerComment GetComment(ChargerComment comment, string docName)
        {
            var comments = db.GetCollection<ChargerComment>(docName);
            var builder = Builders<ChargerComment>.Filter;
            var filter = builder.Eq("user", comment.user) & builder.Eq("statId", comment.statId);

            var docs = comments.Find(filter).ToList();
            if (docs.Count > 0)
                return docs.FirstOrDefault();

            else 
                return null;
        }


        public void InsComment (ChargerComment comment, string docName)
        {
            var document = db.GetCollection<ChargerComment>(docName);
            document.InsertOne(comment);
        }

        public bool DelComment(ChargerComment comment, string docName)
        {
            var comments = db.GetCollection<ChargerComment>(docName);
            var builder = Builders<ChargerComment>.Filter;
            var filter = builder.Eq("user", comment.user) & builder.Eq("statId", comment.statId);

            var del = comments.DeleteOne(filter);

            if(del.DeletedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
