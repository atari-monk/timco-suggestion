using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace SuggestionAppLib.DataAccess;

public class DbConnection : IDbConnection
{
    private readonly IConfiguration config;
    private readonly IMongoDatabase db;
    private const string ConnectionId = "MongoDb";

    public string DbName { get; private set; }
    public string CategoryCollectionName { get; private set; } = "categories";
    public string StatusCollectionName { get; private set; } = "statuses";
    public string UserCollectionName { get; private set; } = "users";
    public string SuggestionsCollectionName { get; private set; } = "suggestions";
    public MongoClient Client { get; private set; }
    public IMongoCollection<CategoryModel> CategoryCollection { get; private set; }
    public IMongoCollection<StatusModel> StatusCollection { get; private set; }
    public IMongoCollection<UserModel> UserCollection { get; private set; }
    public IMongoCollection<SuggestionModel> SuggestionsCollection { get; private set; }

    public DbConnection(IConfiguration config)
    {
        this.config = config;
        Client = new MongoClient(
            config.GetConnectionString(ConnectionId));
        DbName = config["DatabaseName"];
        db = Client.GetDatabase(DbName);
        CategoryCollection = db.GetCollection<CategoryModel>(CategoryCollectionName);
        StatusCollection = db.GetCollection<StatusModel>(StatusCollectionName);
        UserCollection = db.GetCollection<UserModel>(UserCollectionName);
        SuggestionsCollection = db.GetCollection<SuggestionModel>(SuggestionsCollectionName);
    }
}