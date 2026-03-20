using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BriefingApp.Models;

namespace BriefingApp.Data;

public class AppDbContext : IdentityDbContext<AppUser> {
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

}