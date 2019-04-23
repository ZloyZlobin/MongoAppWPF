using System.Linq;
using System.Threading.Tasks;
using MongoAppWPF.Interfaces.DTO.Models;
using MongoAppWPF.Interfaces.Users;
using MongoAppWPF.Interfaces.Users.Service;
using MongoAppWPF.Service.Users;
using Moq;
using Xunit;

namespace MongoAppWPF.Service.Tests.Users
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly IUserService _userService; 

        public UserServiceTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_CallRepository()
        {
            var expectedResult = Enumerable.Empty<User>();
            _repositoryMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(expectedResult);

            var result = await _userService.GetAllUsersAsync();

            Assert.Equal(expectedResult, result);
            _repositoryMock.Verify(x => x.GetAllUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_CallRepository()
        {
            var user = new User();

            await _userService.AddUserAsync(user);

            _repositoryMock.Verify(x => x.AddUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task RemoveUserAsync_Correctly()
        {
            var user = new User();
            _repositoryMock.Setup(x => x.RemoveUserAsync(It.IsAny<User>())).ReturnsAsync(true);

            var result = await _userService.RemoveUserAsync(user);

            Assert.True(result);
            _repositoryMock.Verify(x => x.RemoveUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task RemoveUserAsync_Failed()
        {
            var user = new User();
            _repositoryMock.Setup(x => x.RemoveUserAsync(It.IsAny<User>())).ReturnsAsync(false);

            var result = await _userService.RemoveUserAsync(user);

            Assert.False(result);
            _repositoryMock.Verify(x => x.RemoveUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_Correctly()
        {
            var user = new User();
            _repositoryMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(true);

            var result = await _userService.UpdateUserAsync(user);

            Assert.True(result);
            _repositoryMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_Failed()
        {
            var user = new User();
            _repositoryMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(false);

            var result = await _userService.UpdateUserAsync(user);

            Assert.False(result);
            _repositoryMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task GetUserAsync_FindByName()
        {
            var searchCriteria = "some name";
            var user = new User();
            _repositoryMock.Setup(x => x.GetUserAsync(searchCriteria, SearchOptions.Name)).ReturnsAsync(user);

            var result = await _userService.GetUserAsync(searchCriteria);

            Assert.Equal(user, result);
            _repositoryMock.Verify(x => x.GetUserAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetUserAsync_FindByCountry()
        {
            var searchCriteria = "some country name";
            var user = new User();
            _repositoryMock.Setup(x => x.GetUserAsync(searchCriteria, SearchOptions.Name)).ReturnsAsync((User)null);
            _repositoryMock.Setup(x => x.GetUserAsync(searchCriteria, SearchOptions.Country)).ReturnsAsync(user);

            var result = await _userService.GetUserAsync(searchCriteria);

            Assert.Equal(user, result);
            _repositoryMock.Verify(x => x.GetUserAsync(It.IsAny<string>(), SearchOptions.Name), Times.Once);
            _repositoryMock.Verify(x => x.GetUserAsync(It.IsAny<string>(), SearchOptions.Country), Times.Once);
        }
    }
}
