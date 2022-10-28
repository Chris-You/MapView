using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using MapView.Common.Models.Charger;
using MapView.Common.Models;

namespace MapView.Common.Database
{
    public class Mongo
    {
        private IMongoDatabase db;
        private MongoClient dbClient;

        public Mongo(string user, string host, string dbName)
        {
            dbClient = new MongoClient(string.Format("mongodb://{0}@{1}/{2}",user ,host ,dbName));


            db = dbClient.GetDatabase(dbName);
        }


        public IMongoDatabase GetDatabase(string dbName)
        {
            return dbClient.GetDatabase(dbName);
        }


        public List<ChargerComment> CommentList(string collection)
        {
            var comments = db.GetCollection<ChargerComment>(collection);
            
            var docs = comments.Find(new BsonDocument()).ToList();
            if (docs.Count > 0)
                return docs;
            else
                return null;
        }

        public List<ChargerComment> CommentList(string collection, string statId)
        {
            var comments = db.GetCollection<ChargerComment>(collection);

            var builder = Builders<ChargerComment>.Filter;
            var filter = builder.Eq("statId", statId);

            var docs = comments.Find(filter).ToList();
            if (docs.Count > 0)
                return docs;
            else
                return null;
        }



        public ChargerComment GetComment(ChargerComment comment, string collection)
        {
            var comments = db.GetCollection<ChargerComment>(collection);
            var builder = Builders<ChargerComment>.Filter;
            var filter = builder.Eq("user", comment.user) & builder.Eq("statId", comment.statId);

            var docs = comments.Find(filter).ToList();
            if (docs.Count > 0)
                return docs.FirstOrDefault();

            else 
                return null;

        }


        public void InsComment (ChargerComment comment, string collection)
        {
            var document = db.GetCollection<ChargerComment>(collection);
            document.InsertOne(comment);
        }

        public bool DelComment(ChargerComment comment, string collection)
        {
            var comments = db.GetCollection<ChargerComment>(collection);
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





        public List<T> DataList<T>(string collection)
        {
            var comments = db.GetCollection<T>(collection);

            var docs = comments.Find(new BsonDocument()).ToList();
            if (docs.Count > 0)
                return docs;
            else
                return null;
        }

        public List<T> DataListById<T>(string collection, string contentId)
        {
            var comments = db.GetCollection<T>(collection);

            var builder = Builders<T>.Filter;
            var filter = builder.Eq("contentId", contentId);

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
            var filter = builder.Eq("user", userid) ;

            var docs = doc.Find(filter).ToList();
            if (docs.Count > 0)
                return docs;

            else
                return null;
        }


        public T GetData<T>(string user, ServiceGubun service, string contentId, string collection)
        {
            var doc = db.GetCollection<T>(collection);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("user", user) & builder.Eq("contentId", contentId) & builder.Eq("service", service); 

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

        public bool DelData<T>(string user, ServiceGubun service, string contentId, string collection)
        {
            var doc = db.GetCollection<T>(collection);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("user", user) & builder.Eq("contentId", contentId) & builder.Eq("service", service);

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
