using System;
using System.Threading;
using System.Threading.Tasks;
using MongoAppWPF.External.MongoDB;
using MongoAppWPF.External.Users;
using MongoAppWPF.Interfaces.DTO.Models;
using MongoAppWPF.Interfaces.Exceptions;
using MongoAppWPF.Interfaces.Users;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace MongoAppWPF.External.Tests.Users
{
    public class UserRepositoryTests
    {
        private readonly Mock<IMongoDBClient> _mongoDbClientMock;
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests()
        {
            _mongoDbClientMock = new Mock<IMongoDBClient>();
            _userRepository = new UserRepository(_mongoDbClientMock.Object);
        }

        private Mock<IMongoCollection<User>> CreateMockSubject()
        {
            var settings = new MongoCollectionSettings();
            var mockSubject = new Mock<IMongoCollection<User>> {DefaultValue = DefaultValue.Mock};
            mockSubject.Setup(x => x.DocumentSerializer).Returns(settings.SerializerRegistry.GetSerializer<User>());
            mockSubject.Setup(x => x.Settings).Returns(settings);
            return mockSubject;
        }

        [Fact]
        public async Task GetAllUsersAsync_Correctly()
        {
            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.FindAsync(It.IsAny<ExpressionFilterDefinition<User>>(),
                    It.IsAny<FindOptions<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Mock<IAsyncCursor<User>>().Object);
                _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);
           
            await _userRepository.GetAllUsersAsync();

            mockSubject.Verify(x => x.FindAsync(It.IsAny<ExpressionFilterDefinition<User>>(),
                It.IsAny<FindOptions<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_Failed()
        {
            var exception = new Exception();

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.FindAsync(It.IsAny<ExpressionFilterDefinition<User>>(),
                    It.IsAny<FindOptions<User>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            var thrownException = await Assert.ThrowsAsync<FailedQueryingDataException>(() => _userRepository.GetAllUsersAsync());

            Assert.Equal("GetAllUsersAsync", thrownException.Message);
            Assert.Equal(exception, thrownException.InnerException);
            mockSubject.Verify(x => x.FindAsync(It.IsAny<ExpressionFilterDefinition<User>>(),
                It.IsAny<FindOptions<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_Correctly()
        {
            var user = new User();

            var mockSubject = CreateMockSubject();
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            await _userRepository.AddUserAsync(user);

            Assert.NotNull(user._id);
            mockSubject.Verify(x => x.InsertOneAsync(user, null, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_Failed()
        {
            var exception = new Exception();
            var user = new User();

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.InsertOneAsync(user, null, default(CancellationToken))).ThrowsAsync(exception);
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            var thrownException = await Assert.ThrowsAsync<FailedQueryingDataException>(() => _userRepository.AddUserAsync(user));

            Assert.Equal("AddUserAsync", thrownException.Message);
            Assert.Equal(exception, thrownException.InnerException);
            mockSubject.Verify(x => x.InsertOneAsync(user, null, default(CancellationToken)), Times.Once);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        public async Task RemoveUserAsync_Acknowledged(int deletedCount, bool expectedResult)
        {
            var user = new User();

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(deletedCount));
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            var result = await _userRepository.RemoveUserAsync(user);

            Assert.Equal(expectedResult, result);
            mockSubject.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserAsync_Unacknowledged()
        {
            var user = new User();

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(DeleteResult.Unacknowledged.Instance);
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            var result = await _userRepository.RemoveUserAsync(user);

            Assert.False(result);
            mockSubject.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserAsync_Failed()
        {
            var exception = new Exception();
            var user = new User()
            {
                _id = ObjectId.GenerateNewId()
            };

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            var thrownException = await Assert.ThrowsAsync<FailedQueryingDataException>(() => _userRepository.RemoveUserAsync(user));

            Assert.Equal("RemoveUserAsync", thrownException.Message);
            Assert.Equal(exception, thrownException.InnerException);
            mockSubject.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(2, 2, true)]
        [InlineData(0, 0, false)]
        public async Task UpdateUserAsync_Acknowledged(long matchedCount, long? modifiedCount, bool expectedResult)
        {
            var user = new User();

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<User>(), It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(matchedCount, modifiedCount, null));
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            var result = await _userRepository.UpdateUserAsync(user);

            Assert.Equal(expectedResult, result);
            mockSubject.Verify(
                x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), user,
                    It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_Unacknowledged()
        {
            var user = new User();

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<User>(), It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(ReplaceOneResult.Unacknowledged.Instance);
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            var result = await _userRepository.UpdateUserAsync(user);

            Assert.False(result);
            mockSubject.Verify(
                x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), user,
                    It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_Failed()
        {
            var exception = new Exception();
            var user = new User()
            {
                _id = ObjectId.GenerateNewId()
            };

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<User>(), It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            var thrownException = await Assert.ThrowsAsync<FailedQueryingDataException>(() => _userRepository.UpdateUserAsync(user));

            Assert.Equal("UpdateUserAsync", thrownException.Message);
            Assert.Equal(exception, thrownException.InnerException);
            mockSubject.Verify(
                x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), user,
                    It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUserAsync_WrongOption_Failed()
        {
            var searchCriteria = "Search criteria";
            await Assert.ThrowsAsync<FailedQueryingDataException>(() =>
                _userRepository.GetUserAsync(searchCriteria, (SearchOptions) 4));
        }

        [Fact]
        public async Task GetUserAsync_Failed()
        {
            var searchCriteria = "Search criteria";
            var exception = new Exception();

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            var thrownException = await Assert.ThrowsAsync<FailedQueryingDataException>(() =>
                _userRepository.GetUserAsync(searchCriteria, SearchOptions.Name));

            Assert.Equal("GetUserAsync", thrownException.Message);
            Assert.Equal(exception, thrownException.InnerException);
            mockSubject.Verify(x => x.FindAsync(It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(SearchOptions.Name)]
        [InlineData(SearchOptions.Country)]
        [InlineData(SearchOptions.Age)]
        [InlineData(SearchOptions.ID)]
        public async Task GetUserAsync_Correctly(SearchOptions option)
        {
            var searchCriteria = "Search criteria";
            var exception = new Exception();

            var mockSubject = CreateMockSubject();
            mockSubject.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Mock<IAsyncCursor<User>>().Object);
            _mongoDbClientMock.Setup(x => x.UsersData).Returns(mockSubject.Object);

            await _userRepository.GetUserAsync(searchCriteria, option);

            mockSubject.Verify(x => x.FindAsync(It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
