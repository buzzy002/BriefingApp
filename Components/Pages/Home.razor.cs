using Microsoft.AspNetCore.Components;

namespace BriefingApp.Components.Pages;

public partial class Home {

    [Inject]
    private GeminiAPI GeminiService { get; set; } = default!;
    [Inject]
    private ILogger<Home> Logger { get; set; } = default!;
    private UserPreferences preferences = new UserPreferences();
    private string? response;
    private bool isLoading = false;
    private string newInterest = string.Empty;

    private async Task GetNews() {
        if(preferences.HasNotInterest()) return;
        isLoading = true;
        await GeminiService.FetchNewsAsync(preferences.interests);
        response = GeminiService.GetResponse();
        isLoading = false;
    }

    private void OnPreferedTimeChanged() { Logger.LogInformation($"Prefered time changed to : {preferences.preferedTime}"); }

    private void OnToggleBelgiumNews() { Logger.LogInformation($"Belgium news toggled to {preferences.isBelgiumNewsWanted}"); }

    private void OnToggleWorldNews() { Logger.LogInformation($"World news toggled to {preferences.isWorldNewsWanted}"); }

    private void AddInterest() {
        if(newInterest == string.Empty) return;
        Logger.LogInformation($"New interest added : {newInterest}");
        preferences.AddInterest(newInterest);
        newInterest = string.Empty;
    }

    private void RemoveInterest(string interestToRemove) {
        Logger.LogInformation($"Interest deleted : {interestToRemove}");
        preferences.RemoveInterest(interestToRemove);
    }

}