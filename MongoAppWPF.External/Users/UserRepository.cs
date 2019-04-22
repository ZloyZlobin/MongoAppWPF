using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoAppWPF.External.MongoDB;
using MongoAppWPF.Interfaces.DTO.Models;
using MongoAppWPF.Interfaces.Exceptions;
using MongoAppWPF.Interfaces.Users;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoAppWPF.External.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoDBClient _mongoDbClient;
        public UserRepository(IMongoDBClient mongoDbClient)
        {
            _mongoDbClient = mongoDbClient;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _mongoDbClient.UsersData.Find(u => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new FailedQueryingDataException(nameof(GetAllUsersAsync), ex);
            }
        }

        public async Task<User> GetUserAsync(string searchCriteria, SearchOptions searchOption)
        {
            FilterDefinition<User> filter = null;
            switch (searchOption)
            {
                case SearchOptions.ID:
                    filter = Builders<User>.Filter.Eq(x => x._id, searchCriteria);
                    break;
                case SearchOptions.Name:
                    filter = Builders<User>.Filter.Eq(x => x.NickName, searchCriteria);
                    break;
                case SearchOptions.Age:
                    filter = Builders<User>.Filter.Eq(x => x.Age.ToString(), searchCriteria);
                    break;
                case SearchOptions.Country:
                    filter = Builders<User>.Filter.Eq(x => x.Country, searchCriteria);
                    break;
                default:
                    throw new FailedQueryingDataException("Wrong search criteria");
            }

            try
            {
                return await _mongoDbClient.UsersData.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new FailedQueryingDataException(nameof(GetUserAsync), ex);
            }
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                user._id = ObjectId.GenerateNewId();
                await _mongoDbClient.UsersData.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                throw new FailedQueryingDataException(nameof(AddUserAsync), ex);
            }
        }

        public async Task<bool> RemoveUserAsync(User user)
        {
            try
            {
                DeleteResult actionResult = await _mongoDbClient.UsersData.DeleteOneAsync(Builders<User>.Filter.Eq(x => x._id, user._id));
                return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw new FailedQueryingDataException(nameof(RemoveUserAsync), ex);
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                ReplaceOneResult actionResult = await _mongoDbClient.UsersData.ReplaceOneAsync(u => u._id.Equals(user._id), user, new UpdateOptions{ IsUpsert = true});
                return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw new FailedQueryingDataException(nameof(UpdateUserAsync), ex);
            }
        }
    }
}
