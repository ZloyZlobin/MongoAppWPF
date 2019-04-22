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

        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _repository.GetAllUsersAsync();

        public async Task<User> GetUserAsync(string searchCriteria)
        {

            var result = await _repository.GetUserAsync(searchCriteria, SearchOptions.Name);
            if (result == null)
            {
                result = await _repository.GetUserAsync(searchCriteria, SearchOptions.Country);
            }

            return result;
        }

        public async Task AddUserAsync(User user) => await _repository.AddUserAsync(user);

        public async Task<bool> RemoveUserAsync(User user) => await _repository.RemoveUserAsync(user);

        public async Task<bool> UpdateUserAsync(User user) => await _repository.UpdateUserAsync(user);
    }
}
