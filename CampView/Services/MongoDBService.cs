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

        public List<ChargerComment> CommentList(string docName, string statId)
        {
            var comments = db.GetCollection<ChargerComment>(docName);

            var builder = Builders<ChargerComment>.Filter;
            var filter = builder.Eq("statId", statId);

            var docs = comments.Find(filter).ToList();
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





        public List<T> DataList<T>(string docName)
        {
            var comments = db.GetCollection<T>(docName);

            var docs = comments.Find(new BsonDocument()).ToList();
            if (docs.Count > 0)
                return docs;

            else
                return null;
        }

        public List<T> DataListByStatId<T>(string docName, string statId)
        {
            var comments = db.GetCollection<T>(docName);

            var builder = Builders<T>.Filter;
            var filter = builder.Eq("statId", statId);

            var docs = comments.Find(filter).ToList();
            if (docs.Count > 0)
                return docs;

            else
                return null;
        }

        public List<T> DataListByUser<T>(string docName, string userid)
        {
            var comments = db.GetCollection<T>(docName);

            var builder = Builders<T>.Filter;
            var filter = builder.Eq("user", userid);

            var docs = comments.Find(filter).ToList();
            if (docs.Count > 0)
                return docs;

            else
                return null;
        }


        public T GetData<T>(string user, string statId, string docName)
        {
            var comments = db.GetCollection<T>(docName);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("user", user) & builder.Eq("statId", statId);

            var docs = comments.Find(filter).ToList();
            if (docs.Count > 0)
                return docs.FirstOrDefault();

            else
                return default(T);

        }

        public void InsData<T>(T data, string docName)
        {
            var document = db.GetCollection<T>(docName);
            document.InsertOne(data);
        }

        public bool DelData<T>(string user, string statId, string docName)
        {
            var comments = db.GetCollection<T>(docName);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("user", user) & builder.Eq("statId", statId);

            var del = comments.DeleteOne(filter);

            if (del.DeletedCount > 0)
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
