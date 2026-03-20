using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using BriefingApp.Models;
using BriefingApp.Components;

namespace BriefingApp.Pages.Account;

public class RegisterModel : PageModel {
    
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? Error { get; set; }

    public RegisterModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
        
        _userManager = userManager;
        _signInManager = signInManager;

    }

    public async Task<IActionResult> OnPostAsync() {
        
        AppUser user = new AppUser { UserName = Email, Email = Email };
        var result = await _userManager.CreateAsync(user, Password);

        if(result.Succeeded) {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Redirect("/");
        }

        Error = string.Join(", ", result.Errors.Select(e => e.Description));
        return Page();

    }

}