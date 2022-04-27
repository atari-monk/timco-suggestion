using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLib.DataAccess;

public class MongoStatusData : IStatusData
{
    private readonly IMongoCollection<StatusModel> statuses;
    private readonly IMemoryCache cache;
    private const string CacheName = "StatusData";

    public MongoStatusData(
        IDbConnection db
        , IMemoryCache memoryCache)
    {
        this.cache = memoryCache;
        statuses = db.StatusCollection;
    }

    public async Task<List<StatusModel>> GetAllStatuses()
    {
        var output = cache.Get<List<StatusModel>>(CacheName);
        if (output is null)
        {
            var results = await statuses.FindAsync(_ => true);
            output = results.ToList();

            cache.Set(CacheName, output, TimeSpan.FromDays(1));
        }
        return output;
    }

    public Task CreateStatus(StatusModel status)
    {
        return statuses.InsertOneAsync(status);
    }
}