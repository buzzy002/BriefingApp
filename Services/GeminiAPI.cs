using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;

namespace BriefingApp.Services;

public class GeminiAPI {
    private readonly Client _client;
    private string? _lastResponse;

    public GeminiAPI(IOptions<ApiSettings> apiSettings) {
        string apiKey = apiSettings.Value?.GeminiAPIKey 
                        ?? throw new ArgumentNullException("API Key is missing in appsettings.json");
        
        _client = new Client(apiKey: apiKey);
    }

    public async Task FetchNewsAsync(List<string> interests, bool isBelgiumNewsWanted, bool isWorldNewsWanted) {
        bool hasInterest = interests == null || !interests.Any();
        if (!hasInterest && !isBelgiumNewsWanted && !isWorldNewsWanted) return;

        List<string> sections = new List<string>();
        string yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

        if(isBelgiumNewsWanted) sections.Add($"Find the 3 most important breaking from BELGIUM after:{yesterday}.");
        if(isWorldNewsWanted) sections.Add($"Find the 3 most significant global headlines after:{yesterday}.");
        if(!(interests == null || !interests.Any())) sections.Add($"Find 1 news story after:{yesterday} for: {string.Join(", ", interests)}.");

        string promptText = $"Perform these searches and combine them into one JSON array: {string.Join(". ", sections)}";
        

        try {
            var response = await _client.Models.GenerateContentAsync(
                model: "models/gemini-2.5-flash", 
                contents: new List<Content> { 
                    new Content { Parts = new List<Part> { new Part { Text = promptText } } } 
                },
                config: new GenerateContentConfig {
                    SystemInstruction = new Content { 
                        Parts = new List<Part> { new Part { Text = $@"
                            You are a high-precision news assistant. Today's date is {DateTime.Now:MMMM dd, yyyy}.

                            1. FRESHNESS: Find ONLY ONE news story for each interest. 
                            Every story MUST have been published within the last 24 hours (on or after {DateTime.Now.AddDays(-1):MMMM dd}).
                            2. NEWS SOURCE : major, reputable news organizations (e.g., Reuters, AP, TechCrunch, BBC). 
                            Avoid blogs, social media posts, or login-walled content.
                            3. CANONICAL LINKS: For the 'sourceUrl', you MUST provide the direct, original URL of the news article. 
                            DO NOT use redirection links, proxy links, or URLs starting with 'vertexaisearch.cloud.google.com'. 
                            4. BREVITY: Summaries for BELGIUM and GLOBAL should be 2 sentences. Summaries for specific interests should be 3-4 sentences.
                            5. TOPICS : For Belgium/Global topics, use incremental values like 'Belgium 01', 'Belgium 02'.
                            5. OUTPUT: Return ONLY a valid JSON array.
                            Format: [{{""topic"": """", ""newsTitle"": """", ""summary"": """", ""sourceUrl"": """"}}]

                            If no recent news exists for a topic, omit that topic from the array. 
                            Always provide at least one total item."
                        } } 
                    },
                    Tools = new List<Tool> { new Tool { GoogleSearch = new GoogleSearch() } }
                }
            );

            var candidate = response.Candidates?[0];
            string? rawJson = candidate?.Content?.Parts?[0]?.Text;

            if (!string.IsNullOrEmpty(rawJson)) {
                string jsonPart = ExtractJson(rawJson);
                var articles = JsonSerializer.Deserialize<List<NewsItem>>(jsonPart, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


                if (articles != null) {
                    List<Task> resolveTasks = new List<Task>();

                    foreach (var article in articles) {
                        resolveTasks.Add(Task.Run(async () => {
                            article.sourceUrl = await ResolveLinkAsync(article.sourceUrl);
                        }));
                    }

                    await Task.WhenAll(resolveTasks);
                    _lastResponse = JsonSerializer.Serialize(articles);
                }
            }
        } catch (Exception ex) {
            _lastResponse = $"Error : {ex.Message}";
        }
    }

    public string GetResponse() => _lastResponse ?? "[]";

    public async Task GetAvailablesModelsAsync() {
        var models = await _client.Models.ListAsync();
        await foreach (var m in models) {
            Console.WriteLine($"Available: {m.Name}");
        }
    }

    private async Task<string> ResolveLinkAsync(string redirectURL) {
        
        try {
            using var httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
            var httpResponse = await httpClient.GetAsync(redirectURL);

            string? cleanURL = httpResponse.Headers.Location?.ToString();
            return cleanURL ?? redirectURL;

        } catch {
            return redirectURL;
        }

    }

    private string ExtractJson(string text) {
        
        int start = text.IndexOf('[');
        int end = text.LastIndexOf(']');
        return (start != -1 && end != -1) ? text.Substring(start, (end - start) + 1) : "[]";

    }
}