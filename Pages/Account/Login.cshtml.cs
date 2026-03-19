using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using BriefingApp.Models;

namespace BriefingApp.Pages.Account;

public class LoginModel : PageModel {
    
    private readonly SignInManager<AppUser> _signInManager;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? Error { get; set; }

    public LoginModel(SignInManager<AppUser> signInManager) { _signInManager = signInManager; }

    public async Task<IActionResult> OnPostAsync() {
        
        var result = await _signInManager.PasswordSignInAsync(Email, Password, isPersistent: false, lockoutOnFailure: false);

        if(result.Succeeded) return Redirect("/");

        Error = "Invalid email or password. Please retry.";
        return Page();

    }

}