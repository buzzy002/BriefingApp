public class ApiSettings {
    public string GeminiAPIKey { get; set; }
}

public class NewsItem {
    public string topic { get; set; } = string.Empty;
    public string newsTitle { get; set; } = string.Empty;
    public string summary { get; set; } = string.Empty;
    public string sourceUrl { get; set; } = string.Empty;

}