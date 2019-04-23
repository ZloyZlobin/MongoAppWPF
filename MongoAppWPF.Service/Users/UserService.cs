using System.Collections.Generic;
using System.Threading.Tasks;
using MongoAppWPF.Interfaces.DTO.Models;
using MongoAppWPF.Interfaces.Users;
using MongoAppWPF.Interfaces.Users.Service;

namespace MongoAppWPF.Service.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<User>> GetAllUsersAsync() => _repository.GetAllUsersAsync();

        public async Task<User> GetUserAsync(string searchCriteria)
        {

            var result = await _repository.GetUserAsync(searchCriteria, SearchOptions.Name);
            if (result == null)
            {
                result = await _repository.GetUserAsync(searchCriteria, SearchOptions.Country);
            }

            return result;
        }

        public Task AddUserAsync(User user) => _repository.AddUserAsync(user);
        
        public Task<bool> RemoveUserAsync(User user) => _repository.RemoveUserAsync(user);

        public Task<bool> UpdateUserAsync(User user) => _repository.UpdateUserAsync(user);
    }
}
