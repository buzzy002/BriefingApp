using BriefingApp.Models;

namespace BriefingApp.Tests;

public class UserPreferencesTests {

    [Theory]
    [InlineData("technology")]
    [InlineData("TECHNOLOGY")]
    [InlineData("Technology")]
    public void AddInterest_ShouldBeLower(string input) {

        UserPreferences preferences = new UserPreferences();
        preferences.AddInterest(input);
        Assert.Equal("technology", preferences.interests[0]);

    }

    [Fact]
    public void AddInterest_WhenDuplicate_ShouldNotAdd() {
        
        UserPreferences preferences = new UserPreferences();
        preferences.AddInterest("technology");
        preferences.AddInterest("technology");
        Assert.Single(preferences.interests); 

    }

    [Fact]
    public void RemoveInterest_WhenExisting_ShouldReturnTrue() {
        
        UserPreferences preferences = new UserPreferences();
        preferences.AddInterest("technology");
        bool result = preferences.RemoveInterest("technology");
        Assert.True(result);

    }

    [Fact]
    public void RemoveInterest_WhenNotExisting_ShouldReturnFalse() {
        
        UserPreferences preferences = new UserPreferences();
        bool result = preferences.RemoveInterest("technology");
        Assert.False(result);

    }

    [Fact]
    public void ResetInterest_ShouldEmptyList() {
        
        UserPreferences preferences = new UserPreferences();
        preferences.AddInterest("technology");
        preferences.ResetInterest();
        Assert.Empty(preferences.interests);

    }

    [Fact]
    public void ToggleBelgiumNews_ShouldToggleCorrectly() {
        
        UserPreferences preferences = new UserPreferences();
        Assert.False(preferences.isBelgiumNewsWanted);
        preferences.ToggleBelgiumNews();
        Assert.True(preferences.isBelgiumNewsWanted);
        preferences.ToggleBelgiumNews();
        Assert.False(preferences.isBelgiumNewsWanted);

    }

    [Fact]
    public void ToggleWorldNews_ShouldToggleCorrectly() {
        
        UserPreferences preferences = new UserPreferences();
        Assert.False(preferences.isWorldNewsWanted);
        preferences.ToggleWorldNews();
        Assert.True(preferences.isWorldNewsWanted);
        preferences.ToggleWorldNews();
        Assert.False(preferences.isWorldNewsWanted);

    }

    [Fact]
    public void HasNotInterest_WhenNoInterest_ShouldReturnTrue() {
        
        UserPreferences preferences = new UserPreferences();
        Assert.True(preferences.HasNotInterest());

    }

    [Fact]
    public void HasNotInterest_WhenInterest_ShouldReturnFalse() {
        
        UserPreferences preferences = new UserPreferences();
        preferences.AddInterest("technology");
        Assert.False(preferences.HasNotInterest());

    }

}
