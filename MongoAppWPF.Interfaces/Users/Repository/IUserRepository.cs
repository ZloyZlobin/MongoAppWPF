using System.Collections.Generic;
using System.Threading.Tasks;
using MongoAppWPF.Interfaces.DTO.Models;

namespace MongoAppWPF.Interfaces.Users
{
    public interface IUserRepository
    {
        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Get User by id, name, age, country or any different search criteria
        /// </summary>
        /// <returns></returns>
        Task<User> GetUserAsync(string searchCriteria, SearchOptions searchOption);

        /// <summary>
        /// Add new User
        /// </summary>
        Task AddUserAsync(User user);

        /// <summary>
        /// Remove user
        /// </summary>
        Task<bool> RemoveUserAsync(User user);

        /// <summary>
        /// Update User
        /// </summary>
        Task<bool> UpdateUserAsync(User user);

    }
}
