namespace SuggestionAppUI.Pages;

public partial class SampleData
{
    private bool categoriesCreated = false;
    private bool statusesCreated = false;
    private async Task GenerateSampleData()
    {
        UserModel user = new()
        {FirstName = "Tim", LastName = "Corey", EmailAddress = "tim@test.com", DisplayName = "Sample Tim Corey", ObjectIdentifier = "abc-123"};
        await userData.CreateUser(user);
        var foundUser = await userData.GetUserFromAuthentication("abc-123");
        var categories = await categoryData.GetAllCategories();
        var statuses = await statusData.GetAllStatuses();
        HashSet<string> votes = new();
        votes.Add("1");
        votes.Add("2");
        votes.Add("3");
        SuggestionModel suggestion = new()
        {Author = new BasicUserModel(foundUser), Category = categories[0], Suggestion = "Our First Suggestion", Description = "This is a suggestion created by sample data generation method"};
        await suggestionData.CreateSuggestion(suggestion);
        suggestion = new()
        {Author = new BasicUserModel(foundUser), Category = categories[1], Suggestion = "Our Second Suggestion", Description = "This is a suggestion created by sample data generation method", SuggestionStatus = statuses[0], OwnerNotes = "This is the note for the status"};
        await suggestionData.CreateSuggestion(suggestion);
        suggestion = new()
        {Author = new BasicUserModel(foundUser), Category = categories[2], Suggestion = "Our Third Suggestion", Description = "This is a suggestion created by sample data generation method", SuggestionStatus = statuses[1], OwnerNotes = "This is the note for the status"};
        await suggestionData.CreateSuggestion(suggestion);
        suggestion = new()
        {Author = new BasicUserModel(foundUser), Category = categories[3], Suggestion = "Our Fourth Suggestion", Description = "This is a suggestion created by sample data generation method", SuggestionStatus = statuses[2], UserVotes = votes, OwnerNotes = "This is the note for the status"};
        await suggestionData.CreateSuggestion(suggestion);
        votes.Add("4");
        suggestion = new()
        {Author = new BasicUserModel(foundUser), Category = categories[4], Suggestion = "Our Fifth Suggestion", Description = "This is a suggestion created by sample data generation method", SuggestionStatus = statuses[3], UserVotes = votes, OwnerNotes = "This is the note for the status"};
        await suggestionData.CreateSuggestion(suggestion);
    }

    private async Task CreateCategories()
    {
        var categories = await categoryData.GetAllCategories();
        if (categories?.Count > 0)
        {
            return;
        }

        CategoryModel cat = new()
        {CategoryName = "Courses", CategoryDescription = "Full paid courses"};
        await categoryData.CreateCategory(cat);
        cat = new()
        {CategoryName = "Dev Questions", CategoryDescription = "Advice on developer job"};
        await categoryData.CreateCategory(cat);
        cat = new()
        {CategoryName = "In-Depth Tutorial", CategoryDescription = "A deep-dive video on how to use a topic"};
        await categoryData.CreateCategory(cat);
        cat = new()
        {CategoryName = "10-Minute Training", CategoryDescription = "A quick \"How do i use this\" video"};
        await categoryData.CreateCategory(cat);
        cat = new()
        {CategoryName = "Other", CategoryDescription = "Not sure which category this fits in"};
        await categoryData.CreateCategory(cat);
        categoriesCreated = true;
    }

    private async Task CreateStatuses()
    {
        var statuses = await statusData.GetAllStatuses();
        if (statuses?.Count > 0)
        {
            return;
        }

        StatusModel stat = new()
        {StatusName = "Completed", StatusDescription = "The suggestion was accepted and the corresponding item was created"};
        await statusData.CreateStatus(stat);
        stat = new()
        {StatusName = "Watching", StatusDescription = "The suggestion is interesting. We are watching to see how much interest there is in it"};
        await statusData.CreateStatus(stat);
        stat = new()
        {StatusName = "Upcoming", StatusDescription = "The suggestion was accepted and it will be released soon"};
        await statusData.CreateStatus(stat);
        stat = new()
        {StatusName = "Dismissed", StatusDescription = "The suggestion was not something that we are going to undertake"};
        await statusData.CreateStatus(stat);
        statusesCreated = true;
    }
}