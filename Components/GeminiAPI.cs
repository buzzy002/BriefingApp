using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;

public class GeminiAPI {

    private readonly string? apiKey;
    GenerateContentResponse? APIResponse;
    private readonly IOptions<ApiSettings>? ApiSettings;

    public GeminiAPI() {

        apiKey = ApiSettings.Value.GeminiAPIKey;
        APIResponse = null;
    }

    private async Task SendPrompt(string apiKey, string prompt) {

        var client = new Client(apiKey: apiKey);
        APIResponse = await client.Models.GenerateContentAsync(
            model: "gemini-3-flash-preview", contents: prompt
        );

    }

    public string GetResponse() { return APIResponse.Candidates[0].Content.Parts[0].Text; }

}