using BriefingApp.Models;
using BriefingApp.Services;

namespace BriefingApp.Tests;

public class BriefingFormatterTests {

    private BriefingFormatter _briefingFormatter = new BriefingFormatter();
    
    [Fact]
    public void DisplayTopic_ForBelgium_CheckReturn() {

        string result = _briefingFormatter.ToWebAppHtml(CreateBriefing(topic: "belgium 01"));
        Assert.Contains("Belgium", result);
        Assert.DoesNotContain("belgium 01", result);

    }

    [Fact]
    public void DisplayTopic_ForWorld_CheckReturn() {

        string result = _briefingFormatter.ToWebAppHtml(CreateBriefing(topic: "global 01"));
        Assert.Contains("World", result);
        Assert.DoesNotContain("global 01", result);

    }

    [Fact]
    public void ToWebAppHtml_WhenNoArticles_ShouldReturnEmpty() {
        
        Briefing briefing = new Briefing();
        string result = _briefingFormatter.ToWebAppHtml(briefing);
        Assert.Empty(result);

    }

    [Fact]
    public void ToEmailHtml_WhenNoArticles_ShouldReturnEmpty() {
        
        Briefing briefing = new Briefing();
        string result = _briefingFormatter.ToEmailHtml(briefing);
        Assert.Empty(result);

    }

    [Fact]
    public void ToWebAppHtml_WhenEmptySourceURL_CheckReturn() {

        string result = _briefingFormatter.ToWebAppHtml(CreateBriefing());
        Assert.Contains("Source Unavailable", result);

    }

    [Fact]
    public void ToEmailHtml_WhenEmptySourceURL_CheckReturn() {

        string result = _briefingFormatter.ToEmailHtml(CreateBriefing());
        Assert.Contains("Source Unavailable", result);

    }

    private Briefing CreateBriefing(string topic = "", string newsTitle = "", string summary = "", string sourceUrl = "") {
        
        return new Briefing {
            articles = new List<NewsItem> {
                new NewsItem {
                    topic = topic,
                    newsTitle = newsTitle,
                    summary = summary,
                    sourceUrl = sourceUrl
                }
            }
        };

    }

}