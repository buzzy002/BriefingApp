namespace BriefingApp.Models;

public class UserPreferences {
    
    public int Id { get; set; }
    public List<string> interests { get; set; }
    public TimeOnly preferedTime {get; set;}
    public bool isBelgiumNewsWanted {get; set;}
    public bool isWorldNewsWanted {get; set;}
    
    public UserPreferences() {
        
        interests = new List<string>();
        preferedTime = new TimeOnly(7,0);

    }

    public bool AddInterest(string newInterest) {
        
        if(!interests.Contains(newInterest.ToLower())) {
            interests.Add(newInterest.ToLower());
            return true;
        }
        return false;

    }

    public bool RemoveInterest(string interestToRemove) { return interests.Remove(interestToRemove.ToLower()); }

    public void ResetInterest() => interests.Clear();

    public void ToggleBelgiumNews() => isBelgiumNewsWanted = !isBelgiumNewsWanted;
    public void ToggleWorldNews() => isWorldNewsWanted = !isWorldNewsWanted;

    public bool HasNotInterest() { return interests.Count == 0; }

}