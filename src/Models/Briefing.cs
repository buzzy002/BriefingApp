namespace BriefingApp.Models;

public class NewsItem {
    public string topic { get; set; } = string.Empty;
    public string newsTitle { get; set; } = string.Empty;
    public string summary { get; set; } = string.Empty;
    public string sourceUrl { get; set; } = string.Empty;

}

public class Briefing {
    public List<NewsItem>? articles { get; set; } = new List<NewsItem>()!;
    public DateTime fetchedAt { get; set; }
}