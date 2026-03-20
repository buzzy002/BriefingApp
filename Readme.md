# BriefingApp

A personalized AI-powered daily news briefing web app built with Blazor Server and .NET 10.

## What it does

BriefingApp delivers a daily news briefing tailored to your interests, sent directly to your email at your preferred time. News is fetched using the Gemini API with Google Search grounding to ensure fresh, real results.

## Features

- 🔐 User authentication (register, login, logout)
- 📰 Personalized news based on custom interests
- 🇧🇪 Optional Belgian news section
- 🌍 Optional World news section
- ⏰ Scheduled daily email briefing at your preferred time
- 🔑 Per-user Gemini API key (stored encrypted)
- 👁️ Briefing preview in the web app
- ⚙️ User preferences persisted in database

## Tech stack

- **Blazor Server** (.NET 10)
- **ASP.NET Core Identity** for authentication
- **Entity Framework Core** with SQLite
- **Google Gemini API** (gemini-2.5-flash) for news fetching
- **MailKit** for email delivery via Gmail SMTP
- **AES encryption** for API key storage
- **xUnit** for unit tests
- **Bootstrap 5** for UI

## Project structure
```
BriefingApp/
├── src/                      # Main Blazor Server project
│   ├── Components/Pages/     # Blazor pages
│   ├── Models/               # AppUser, UserPreferences, Briefing, NewsItem
│   ├── Services/             # GeminiAPI, EmailService, BriefingFormatter, EncryptionService, BriefingScheduler
│   ├── Data/                 # AppDbContext
│   └── Pages/Account/        # Razor Pages for auth (login, register, logout)
└── BriefingApp.Tests/        # xUnit test project
```

## Getting started

### Prerequisites
- .NET 10 SDK
- A Google Gemini API key
- A Gmail account with an App Password

### Setup

1. Clone the repository
```bash
git clone https://github.com/buzzy002/BriefingApp.git
cd BriefingApp
```

2. Set user secrets
```bash
cd src
dotnet user-secrets set "Encryption:Key" "your-secret-key"
dotnet user-secrets set "Gmail:Email" "your@gmail.com"
dotnet user-secrets set "Gmail:AppPassword" "your-app-password"
```

3. Run migrations
```bash
dotnet ef database update
```

4. Run the app
```bash
dotnet run
```

5. Register an account, add your Gemini API key in Settings, set your interests and preferred time.

## Running tests
```bash
cd BriefingApp.Tests
dotnet test
```