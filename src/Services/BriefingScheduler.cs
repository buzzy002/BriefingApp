using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using BriefingApp.Data;
using BriefingApp.Models;

namespace BriefingApp.Services;

public class BriefingScheduler : BackgroundService {
    
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BriefingScheduler> _logger;

    public BriefingScheduler(IServiceProvider serviceProvider, ILogger<BriefingScheduler> logger) {
        
        _serviceProvider = serviceProvider;
        _logger = logger;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        
        while(!stoppingToken.IsCancellationRequested) {
            await CheckAndSendBriefingAsync();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

    }

    private async Task CheckAndSendBriefingAsync() {
        
        using var scope = _serviceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        GeminiAPI geminiService = scope.ServiceProvider.GetRequiredService<GeminiAPI>();
        EmailService emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
        EncryptionService encryptionService = scope.ServiceProvider.GetRequiredService<EncryptionService>();
        BriefingFormatter briefingFormatter = scope.ServiceProvider.GetRequiredService<BriefingFormatter>();

        TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

        List<AppUser> users = await dbContext.Users
            .Include(u => u.preferences)
            .ToListAsync();

        foreach (AppUser user in users) {
            
            if(user.Email == null || user.EncryptedGeminiApiKey == null) continue;
            if(user.preferences.HasNotInterest() && !user.preferences.isBelgiumNewsWanted && !user.preferences.isWorldNewsWanted) continue;

            TimeOnly userPreferedTime = user.preferences.preferedTime;
            if(now.Hour != userPreferedTime.Hour || now.Minute != userPreferedTime.Minute) continue;

            _logger.LogInformation($"Sending briefing to {user.Email}");

            try {
                string apiKey = encryptionService.Decrypt(user.EncryptedGeminiApiKey);
                Briefing briefing = await geminiService.FetchNewsAsync(user.preferences.interests, user.preferences.isBelgiumNewsWanted, user.preferences.isWorldNewsWanted, apiKey);

                if(briefing.articles == null) throw new Exception("No article");
                await emailService.SendBriefingAsync(user.Email, briefingFormatter.ToEmailHtml(briefing));
                _logger.LogInformation($"Briefing sent to {user.Email}");
            } catch (Exception ex) {
                _logger.LogError($"Failed to send briefing to {user.Email} : {ex.Message}");
            }

        }

    }

}