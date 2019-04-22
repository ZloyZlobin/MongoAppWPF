using MongoAppWPF.Interfaces.DTO.Models;
using MongoDB.Driver;

namespace MongoAppWPF.External.MongoDB
{
    public interface IMongoDBClient
    {
        IMongoCollection<User> UsersData { get; }
    }
}
