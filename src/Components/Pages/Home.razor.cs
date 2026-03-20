using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using BriefingApp.Models;
using BriefingApp.Services;
using BriefingApp.Data;

namespace BriefingApp.Components.Pages;

public partial class Home {

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private UserManager<AppUser> UserManager { get; set; } = default!;
    [Inject] private AppDbContext DbContext { get; set; } = default!;
    [Inject] private GeminiAPI GeminiService { get; set; } = default!;
    [Inject] private EncryptionService EncryptionService { get; set; } = default!;
    [Inject] private BriefingFormatter BriefingFormatter { get; set; } = default!;
    [Inject] private ILogger<Home> _logger { get; set; } = default!;

    private AppUser? _currentUser;
    private UserPreferences _preferences = new UserPreferences();
    private string? _response;
    private bool _isLoading = false;
    private string _newInterest = string.Empty;
    private CancellationTokenSource? _preferedTimeCts;

    protected override async Task OnInitializedAsync() {
        
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var claimUser = authState.User;

        if(claimUser.Identity?.IsAuthenticated == true) {
            _currentUser = await UserManager.Users
                .Include(u => u.preferences)
                .FirstOrDefaultAsync(u => u.UserName == claimUser.Identity.Name);

            if(_currentUser != null) _preferences = _currentUser.preferences;
        };

    }

    private async Task GetBriefingExample() {
        if(_preferences.HasNotInterest() && !_preferences.isBelgiumNewsWanted && !_preferences.isWorldNewsWanted) return;
        if(_currentUser?.EncryptedGeminiApiKey == null) {
            _response = "Please set your Gemini API key in Settings";
            return;
        }

        _isLoading = true;
        string apiKey = EncryptionService.Decrypt(_currentUser.EncryptedGeminiApiKey);
        Briefing briefing = await GeminiService.FetchNewsAsync(_preferences.interests, _preferences.isBelgiumNewsWanted, _preferences.isWorldNewsWanted, apiKey);
        _logger.LogInformation($"Articles count: {briefing.articles.Count}");
        foreach (var a in briefing.articles) { _logger.LogInformation($"Topic: {a.topic} | Title: {a.newsTitle}"); }
        _response = BriefingFormatter.ToWebAppHtml(briefing);
        _isLoading = false;
    }

    private async Task SavePreferences() { await DbContext.SaveChangesAsync(); }

    private async void OnPreferedTimeChanged() { 
        _preferedTimeCts?.Cancel();
        _preferedTimeCts = new CancellationTokenSource();

        try {
            await Task.Delay(1000, _preferedTimeCts.Token);
            await SavePreferences();
            _logger.LogInformation($"Prefered time changed to : {_preferences.preferedTime}"); 
        } catch (TaskCanceledException) {}
    }

    private async void OnToggleBelgiumNews() {
        await SavePreferences();
        _logger.LogInformation($"Belgium news toggled to {_preferences.isBelgiumNewsWanted}"); 
    }

    private async void OnToggleWorldNews() {
        await SavePreferences();
        _logger.LogInformation($"World news toggled to {_preferences.isWorldNewsWanted}"); 
    }

    private async void AddInterest() {
        if(_newInterest == string.Empty) return;
        _logger.LogInformation($"New interest added : {_newInterest}");
        _preferences.AddInterest(_newInterest);
        _newInterest = string.Empty;
        await SavePreferences();
    }

    private async void RemoveInterest(string interestToRemove) {
        _logger.LogInformation($"Interest deleted : {interestToRemove}");
        _preferences.RemoveInterest(interestToRemove);
        await SavePreferences();
    }

    private async void ResetInterest() {
        _logger.LogInformation($"Interests reseted");
        _preferences.ResetInterest();
        await SavePreferences();
    }

}