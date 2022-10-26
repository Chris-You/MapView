using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using MapView.Common.Models.Webtoon;

namespace MapView.Common.Database
{
    public class MongoDBWebtoon
    {
        private IMongoDatabase db;
        private MongoClient dbClient;

        public MongoDBWebtoon(string user, string host, string dbName)
        {
            dbClient = new MongoClient(string.Format("mongodb://{0}@{1}/{2}",user ,host ,dbName));


            db = dbClient.GetDatabase(dbName);
        }


        public IMongoDatabase GetDatabase(string dbName)
        {
            return dbClient.GetDatabase(dbName);
        }


        public List<T> DataList<T>(string collection)
        {
            var comments = db.GetCollection<T>(collection);

            var docs = comments.Find(new BsonDocument()).ToList();
            if (docs.Count > 0)
                return docs;
            else
                return null;
        }

        public List<T> DataListByTitleId<T>(string collection, int titleId)
        {
            var comments = db.GetCollection<T>(collection);

            var builder = Builders<T>.Filter;
            var filter = builder.Eq("titleId", titleId);

            var docs = comments.Find(filter).ToList();
            if (docs.Count > 0)
                return docs;

            else
                return null;
        }

        public List<T> DataListByUser<T>(string collection, string userid)
        {
            var doc = db.GetCollection<T>(collection);

            var builder = Builders<T>.Filter;
            var filter = builder.Eq("user", userid);

            var docs = doc.Find(filter).ToList();
            if (docs.Count > 0)
                return docs;

            else
                return null;
        }


        public T GetData<T>(int titleId, string collection)
        {
            var doc = db.GetCollection<T>(collection);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("titleId", titleId);

            var docs = doc.Find(filter).ToList();
            if (docs.Count > 0)
                return docs.FirstOrDefault();

            else
                return default(T);

        }

        public T GetData<T>(int titleId, int no, string collection)
        {
            var doc = db.GetCollection<T>(collection);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("titleId", titleId) & builder.Eq("no", no);

            var docs = doc.Find(filter).ToList();
            if (docs.Count > 0)
                return docs.FirstOrDefault();

            else
                return default(T);

        }


        public void InsData<T>(T data, string collection)
        {
            var document = db.GetCollection<T>(collection);
            document.InsertOne(data);
        }

        public bool DelData<T>(int titleId, string collection)
        {
            var doc = db.GetCollection<T>(collection);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("titleId", titleId);

            var del = doc.DeleteOne(filter);

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
