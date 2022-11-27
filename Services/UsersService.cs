using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersService(
            IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                bookStoreDatabaseSettings.Value.UsersCollectionName);
        }

        public async Task CreateAsync(User newUser) => await _usersCollection.InsertOneAsync(newUser);

        public async Task<User?> GetAsync(string email) => await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }
}
