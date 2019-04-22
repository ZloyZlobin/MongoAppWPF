using System;
using MongoAppWPF.Interfaces.DTO.Models;
using MongoAppWPF.Interfaces.Exceptions;
using MongoDB.Driver;

namespace MongoAppWPF.External.MongoDB
{
    public class MongoDBClient : IMongoDBClient
    {
        private readonly IMongoDatabase _database;

        public MongoDBClient(MongoDBConfig config)
        {
            try
            {
                var client = new MongoClient(config.Connection);
                _database = client.GetDatabase(config.Database);
            }
            catch (Exception ex)
            {
                throw new FailedConnectingToDBException($"Failed connecting to MongoDB({config.Database})", ex);
            }
            
        }

        public IMongoCollection<User> UsersData => _database?.GetCollection<User>("Users");
    }
}
