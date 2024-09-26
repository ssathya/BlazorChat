using Microsoft.SemanticKernel;
using SimpleFeedReader;
using System.ComponentModel;

namespace BlazorChat.Services;

public class NewsPlugin
{
    [KernelFunction("get_news")]
    [Description("Gets news item for today's date")]
    [return: Description("A list of current news stories.")]
    public List<FeedItem> GetNews(Kernel kernel, string category)
    {
        if (category.ToLower().Trim().Contains("head") ||
            category.ToLower().Trim().Equals("home"))
        {
            category = "homepage";
        }
        // Get the current date)
        var reader = new FeedReader();
        IEnumerable<FeedItem> feedItems = reader.RetrieveFeed($"https://rss.nytimes.com/services/xml/rss/nyt/{category}.xml")
            .Take(6);
        return feedItems.ToList();
    }

    [KernelFunction("news_categories")]
    [Description("Gets a list of news categories that we can source")]
    public List<string> GetCategories(Kernel kernel)
    {
        List<string> categories = [
            "Headline", "World", "Business","Technology", "Sports","Science","Health"
            ];
        return categories;
    }
}