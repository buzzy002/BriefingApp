using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;

using BriefingApp.Models;

namespace BriefingApp.Services;

public class GeminiAPI {

    private readonly ILogger<GeminiAPI> _logger;

    public GeminiAPI(ILogger<GeminiAPI> logger) { _logger = logger; }

    public async Task<Briefing> FetchNewsAsync(List<string> interests, bool isBelgiumNewsWanted, bool isWorldNewsWanted, string apiKey) {

        var _client = new Client(apiKey: apiKey);

        bool hasInterest = interests == null || !interests.Any();
        if (!hasInterest && !isBelgiumNewsWanted && !isWorldNewsWanted) return new Briefing();

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
                            if you are not 100% certain the URL exists and is accessible, leave it empty rather than guessing. NEVER construct or infer URLs.
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
                            article.sourceUrl = await ResolveAndValidateLinkAsync(article.sourceUrl);
                        }));
                    }

                    await Task.WhenAll(resolveTasks);
                    return new Briefing { articles = articles, fetchedAt = DateTime.Now };
                }
            }
            return new Briefing();
        } catch (Exception ex) {
            _logger.LogError($"{ex.Message}");
            return new Briefing();
        }
    }

    public async Task GetAvailablesModelsAsync(string apiKey) {
        var _client = new Client(apiKey: apiKey);

        var models = await _client.Models.ListAsync();
        await foreach (var m in models) {
            Console.WriteLine($"Available: {m.Name}");
        }
    }

    private async Task<string> ResolveAndValidateLinkAsync(string url) {

        string domainFallback;
        try {
            Uri uri = new Uri(url);
            domainFallback = $"{uri.Scheme}://{uri.Host}";
        } catch { return string.Empty; }
        
        try {
            using var httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
            var httpResponse = await httpClient.GetAsync(url);

            if(httpResponse.Headers.Location != null) return httpResponse.Headers.Location.ToString();

            if(httpResponse.IsSuccessStatusCode) return url;

            return domainFallback;
        } catch { return domainFallback; }

    }

    private string ExtractJson(string text) {

        text = text.Replace("```json", "").Replace("```", "").Trim();
        
        int start = text.IndexOf('[');
        int end = text.LastIndexOf(']');
        return (start != -1 && end != -1) ? text.Substring(start, (end - start) + 1) : "[]";

    }
}