using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLib.DataAccess;

public class MongoCategoryData 
    : ICategoryData
{
    private readonly IMongoCollection<CategoryModel> categories;
    private readonly IMemoryCache cache;
    private const string CacheName = "CategoryData";

    public MongoCategoryData(
        IDbConnection db
        , IMemoryCache memoryCache)
    {
        this.cache = memoryCache;
        categories = db.CategoryCollection;
    }

    public async Task<List<CategoryModel>> GetAllCategories()
    {
        var output = cache.Get<List<CategoryModel>>(CacheName);
        if (output is null)
        {
            var results = await categories.FindAsync(_ => true);
            output = results.ToList();

            cache.Set(CacheName, output, TimeSpan.FromDays(1));
        }
        return output;
    }

    public Task CreateCategory(CategoryModel category)
    {
        return categories.InsertOneAsync(category);
    }
}