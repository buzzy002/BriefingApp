using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

using BriefingApp.Models;
using BriefingApp.Data;
using BriefingApp.Services;
using System.Runtime.InteropServices;

namespace BriefingApp.Components.Pages;

public partial class Settings {
    
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private UserManager<AppUser> UserManager { get; set; } = default!;
    [Inject] private EncryptionService EncryptionService { get; set; } = default!;

    private AppUser? _currentUser;
    private string _apiKey = string.Empty;
    private string? _message;
    private bool _showKey = false;

    protected override async Task OnInitializedAsync() {
        
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var claimUser = authState.User;

        if(claimUser.Identity?.IsAuthenticated == true) {
            _currentUser = await UserManager.GetUserAsync(claimUser);

            if(_currentUser?.EncryptedGeminiApiKey != null) _apiKey = EncryptionService.Decrypt(_currentUser.EncryptedGeminiApiKey);
        }

    }

    private async Task SaveApiKey() {
        
        if(_currentUser == null || string.IsNullOrEmpty(_apiKey)) return;

        _currentUser.EncryptedGeminiApiKey = EncryptionService.Encrypt(_apiKey);
        await UserManager.UpdateAsync(_currentUser);
        _message = "API key saved successfully !";

    }

    private void ToggleShowKey() => _showKey = !_showKey;

}