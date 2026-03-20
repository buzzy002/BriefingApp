using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using BriefingApp.Pages.Account;
using BriefingApp.Models;

namespace BriefingApp.Pages.Account;

public class LogoutModel : PageModel {
    
    private readonly SignInManager<AppUser> _signInManager;

    public LogoutModel(SignInManager<AppUser> signInManager) { _signInManager = signInManager; }

    public async Task<IActionResult> OnGetAsync() {
        
        await _signInManager.SignOutAsync();
        return Redirect("/");

    }

}