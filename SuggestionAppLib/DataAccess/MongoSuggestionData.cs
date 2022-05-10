using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLib.DataAccess;

public class MongoSuggestionData 
    : ISuggestionData
{
    private readonly IDbConnection dblink;
    private readonly IUserData userData;
    private readonly IMemoryCache cache;
    private readonly IMongoCollection<SuggestionModel> suggestions;
    private const string CacheName = "SuggestionData";

    public MongoSuggestionData(
        IDbConnection db
        , IUserData userData
        , IMemoryCache memoryCache)
    {
        this.dblink = db;
        this.userData = userData;
        this.cache = memoryCache;
        suggestions = db.SuggestionsCollection;
    }

    public async Task<List<SuggestionModel>> GetAllSuggestions()
    {
        var output = cache.Get<List<SuggestionModel>>(CacheName);
        if (output is null)
        {
            var results = await suggestions.FindAsync(s => s.Archived == false);
            output = results.ToList();

            cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    public async Task<List<SuggestionModel>> GetUsersSuggestions(string userId)
    {
        var output = cache.Get<List<SuggestionModel>>(userId);
        if (output is null)
        {
            var results = await suggestions.FindAsync(s => s.Author.Id == userId);
            output = results.ToList();

            cache.Set(userId, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }
     
    public async Task<List<SuggestionModel>> GetAllApprovedSuggestions()
    {
        var output = await GetAllSuggestions();
        return output.Where(s => s.ApprovedForRelease).ToList();
    }

    public async Task<SuggestionModel> GetSuggestion(string id)
    {
        var results = await suggestions.FindAsync(u => u.Id == id);
        return results.FirstOrDefault();
    }

    public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval()
    {
        var output = await GetAllSuggestions();
        return output.Where(s =>
            s.ApprovedForRelease == false
            && s.Rejected == false).ToList();
    }

    public async Task UpdateSuggestion(SuggestionModel suggestion)
    {
        await suggestions.ReplaceOneAsync(
            s => s.Id == suggestion.Id
            , suggestion);
        cache.Remove(CacheName);
    }

    public async Task UpvoteSuggestion(
        string suggestionId
        , string userId)
    {
        var client = dblink.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(dblink.DbName);

            var suggestionsInTransaction =
                db.GetCollection<SuggestionModel>(
                    dblink.SuggestionsCollectionName);
            var suggestion =
                (await suggestionsInTransaction.FindAsync(
                    s => s.Id == suggestionId)).First();
            var isUpvote = suggestion.UserVotes.Add(userId);
            if (isUpvote == false)
            {
                suggestion.UserVotes.Remove(userId);
            }
            await suggestionsInTransaction.ReplaceOneAsync(
                s => s.Id == suggestionId
                , suggestion);

            var usersInTransaction =
                db.GetCollection<UserModel>(
                    dblink.UserCollectionName);
            var user = await userData.GetUser(suggestion.Author.Id);
            if (isUpvote)
            {
                user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
            }
            else
            {
                var suggestionToRemove = user.VotedOnSuggestions.Where(
                    s => s.Id == suggestionId).First();
                user.VotedOnSuggestions.Remove(suggestionToRemove);
            }
            await usersInTransaction.ReplaceOneAsync(u => u.Id == userId, user);

            await session.CommitTransactionAsync();
            cache.Remove(CacheName);
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public async Task CreateSuggestion(SuggestionModel suggestion)
    {
        var client = dblink.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(dblink.DbName);

            var suggestionsInTransaction =
                db.GetCollection<SuggestionModel>(
                    dblink.SuggestionsCollectionName);
            await suggestionsInTransaction.InsertOneAsync(suggestion);

            var usersInTransaction =
                db.GetCollection<UserModel>(
                    dblink.UserCollectionName);
            var user = await userData.GetUser(suggestion.Author.Id);
            user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
            await usersInTransaction.ReplaceOneAsync(u => u.Id == user.Id, user);

            await session.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}