using Microsoft.AspNetCore.Components;
using BriefingApp.Models;
using BriefingApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BriefingApp.Components.Pages;

public partial class Home {

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private UserManager<AppUser> UserManager { get; set; } = default!;
    [Inject] private GeminiAPI GeminiService { get; set; } = default!;
    [Inject] private ILogger<Home> Logger { get; set; } = default!;

    private AppUser? currentUser;
    private UserPreferences preferences = new UserPreferences();
    private string? response;
    private bool isLoading = false;
    private string newInterest = string.Empty;

    protected override async Task OnInitializedAsync() {
        
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var claimUser = authState.User;

        if(claimUser.Identity?.IsAuthenticated == true) currentUser = await UserManager.GetUserAsync(claimUser);

    }

    private async Task GetNews() {
        if(preferences.HasNotInterest()) return;
        isLoading = true;
        await GeminiService.FetchNewsAsync(preferences.interests, preferences.isBelgiumNewsWanted, preferences.isWorldNewsWanted);
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