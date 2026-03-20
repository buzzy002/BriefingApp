using Microsoft.AspNetCore.Components;
using BriefingApp.Models;
using BriefingApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using BriefingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BriefingApp.Components.Pages;

public partial class Home {

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private UserManager<AppUser> UserManager { get; set; } = default!;
    [Inject] private AppDbContext DbContext { get; set; } = default!;
    [Inject] private GeminiAPI GeminiService { get; set; } = default!;
    [Inject] private EncryptionService EncryptionService { get; set; } = default!;
    [Inject] private ILogger<Home> Logger { get; set; } = default!;

    private AppUser? currentUser;
    private UserPreferences preferences = new UserPreferences();
    private string? response;
    private bool isLoading = false;
    private string newInterest = string.Empty;

    protected override async Task OnInitializedAsync() {
        
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var claimUser = authState.User;

        if(claimUser.Identity?.IsAuthenticated == true) {
            currentUser = await UserManager.Users
                .Include(u => u.preferences)
                .FirstOrDefaultAsync(u => u.UserName == claimUser.Identity.Name);

            if(currentUser != null) preferences = currentUser.preferences;
        };

    }

    private async Task GetNews() {
        if(preferences.HasNotInterest()) return;
        if(currentUser?.EncryptedGeminiApiKey == null) {
            response = "Please set your Gemini API key in Settings";
            return;
        }


        isLoading = true;
        string apiKey = EncryptionService.Decrypt(currentUser.EncryptedGeminiApiKey);
        await GeminiService.FetchNewsAsync(preferences.interests, preferences.isBelgiumNewsWanted, preferences.isWorldNewsWanted, apiKey);
        response = GeminiService.GetResponse();
        isLoading = false;
    }

    private async Task SavePreferences() { await DbContext.SaveChangesAsync(); }

    private async void OnPreferedTimeChanged() { 
        await SavePreferences();
        Logger.LogInformation($"Prefered time changed to : {preferences.preferedTime}"); 
    }

    private async void OnToggleBelgiumNews() {
        await SavePreferences();
        Logger.LogInformation($"Belgium news toggled to {preferences.isBelgiumNewsWanted}"); 
    }

    private async void OnToggleWorldNews() {
        await SavePreferences();
        Logger.LogInformation($"World news toggled to {preferences.isWorldNewsWanted}"); 
    }

    private async void AddInterest() {
        if(newInterest == string.Empty) return;
        Logger.LogInformation($"New interest added : {newInterest}");
        preferences.AddInterest(newInterest);
        newInterest = string.Empty;
        await SavePreferences();
    }

    private async void RemoveInterest(string interestToRemove) {
        Logger.LogInformation($"Interest deleted : {interestToRemove}");
        preferences.RemoveInterest(interestToRemove);
        await SavePreferences();
    }

}