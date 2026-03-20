using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BriefingApp.Services;

public class EmailService {
    
    private readonly string _email;
    private readonly string _appPassword;

    public EmailService(IConfiguration configuration) {
        
        _email = configuration["Gmail:Email"] ?? throw new Exception("Gmail email not found");
        _appPassword = configuration["Gmail:AppPassword"] ?? throw new Exception("Gmail app password not found");

    }

    public async Task SendBriefingAsync(string toEmail, string briefingContent) {
        
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("BriefingApp", _email));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = $"Your daily briefing - {DateTime.Now:dd MMMM yyyy}";

        message.Body = new TextPart("html") { Text = briefingContent };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_email, _appPassword);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);

    }

}