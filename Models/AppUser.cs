using BriefingApp.Components;
using Microsoft.AspNetCore.Identity;

namespace BriefingApp.Models;

public class AppUser : IdentityUser {
    
    public UserPreferences preferences { get; set; } = new UserPreferences();
    public string? EncryptedGeminiApiKey { get; set; }

}