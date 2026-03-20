using System.Text;

using BriefingApp.Models;

namespace BriefingApp.Services;

public class BriefingFormatter {
    
    public string ToEmailHtml(Briefing briefing) {

        if(briefing.articles == null || briefing.articles.Count == 0) return string.Empty;
        
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"<h1>Your Briefing of {briefing.fetchedAt:dd/MM/yyyy}</h1>");

        foreach(NewsItem article in briefing.articles) {

            string formattedTopic = DisplayTopic(article.topic);
            
            stringBuilder.Append($@"
                <div style=""margin-bottom: 3rem"">
                    <h3>[{formattedTopic}] <span style=""text-decoration: underline""> {article.newsTitle}</span></h3>
                    <p>{article.summary}</p>
            ");

            if(article.sourceUrl == string.Empty) stringBuilder.Append($"<p style=\"font-style: italic\"><strong>Source Unavailable</strong></p>");
            else stringBuilder.Append($"<a style=\"font-style: italic\" href=\"{article.sourceUrl}\" target=\"_blank\">Source URL</a>");

            stringBuilder.Append("</div>");

        }

        return stringBuilder.ToString();

    }

    public string ToWebAppHtml(Briefing briefing) {

        if(briefing.articles == null || briefing.articles.Count == 0) return string.Empty;
        
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"<h1>Example of briefing");

        foreach(NewsItem article in briefing.articles) {

            string formattedTopic = DisplayTopic(article.topic);
            
            stringBuilder.Append($@"
                <div class=""my-4"">
                    <h4>[{formattedTopic}] <span class=""text-decoration-underline mb-2""> {article.newsTitle}</span></h4>
                    <p class=""fs-5"" style=""margin-bottom: -1rem"">{article.summary}</p>
            ");

            if(article.sourceUrl == string.Empty) stringBuilder.Append($"<p class=\"fw-bold fst-italic fs-5\">Source Unavailable</p>");
            else stringBuilder.Append($"<a class=\"fst-italic fs-5\" href=\"{article.sourceUrl}\" target=\"_blank\">Source URL</a>");

            stringBuilder.Append("</div>");

        }

        return stringBuilder.ToString();

    }

    private string DisplayTopic(string topic) {
        
        string formattedTopic = topic.ToLower() switch {
            string t when t.Contains("belgium") => "Belgium",
            string t when t.Contains("global") => "World",
            _ => topic
        };
        return formattedTopic;

    }

}