using MongoDB.Driver;

namespace SuggestionAppLib.DataAccess;

public interface IDbConnection
{
    string DbName { get; }
    string CategoryCollectionName { get; }
    string StatusCollectionName { get; }
    string UserCollectionName { get; }
    string SuggestionsCollectionName { get; }
    MongoClient Client { get; }
    IMongoCollection<CategoryModel> CategoryCollection { get; }
    IMongoCollection<StatusModel> StatusCollection { get; }
    IMongoCollection<UserModel> UserCollection { get; }
    IMongoCollection<SuggestionModel> SuggestionsCollection { get; }
}
