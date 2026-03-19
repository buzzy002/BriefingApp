using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;

namespace BriefingApp.Components;

public class GeminiAPI {
    private readonly Client _client;
    private string? _lastResponse;

    public GeminiAPI(IOptions<ApiSettings> apiSettings) {
        string apiKey = apiSettings.Value?.GeminiAPIKey 
                        ?? throw new ArgumentNullException("API Key is missing in appsettings.json");
        
        _client = new Client(apiKey: apiKey);
    }

    public async Task FetchNewsAsync(List<string> interests) {
        if (interests == null || !interests.Any()) return;

        string promptText = $"Find today's news for: {string.Join(", ", interests)}";

        try {
            var response = await _client.Models.GenerateContentAsync(
                model: "models/gemini-2.5-flash-lite", 
                contents: new List<Content> { 
                    new Content { Parts = new List<Part> { new Part { Text = promptText } } } 
                },
                config: new GenerateContentConfig {
                    SystemInstruction = new Content { 
                        Parts = new List<Part> { new Part { Text = @"
                            You are a news assistant. 
                            Use Google Search to find one story for each interest.
                            Summaries must be 3-4 sentences. 
                            Output ONLY a valid JSON array. 
                            Format: [{""topic"": """", ""newsTitle"": """", ""summary"": """", ""sourceUrl"": """"}]
                            Never return an empty array []. Always provide at least one news item.
                            If no news found, write ""no news found"""
                        } } 
                    },
                    Tools = new List<Tool> { new Tool { GoogleSearch = new GoogleSearch() } }
                }
            );

            string? rawText = response.Candidates?[0].Content?.Parts?[0]?.Text;

            if (!string.IsNullOrEmpty(rawText)) {
                int start = rawText.IndexOf('[');
                int end = rawText.LastIndexOf(']');

                if (start != -1 && end != -1 && end > start) {
                    _lastResponse = rawText.Substring(start, (end - start) + 1);
                } else {
                    _lastResponse = "[]"; 
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
}