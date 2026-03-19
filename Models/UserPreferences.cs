namespace BriefingApp.Models;

public class UserPreferences {
    
    public int Id { get; set; }
    public List<string> interests { get; set; }
    public TimeOnly preferedTime {get; set;}
    public bool isBelgiumNewsWanted;
    public bool isWorldNewsWanted;
    
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

    public void ResetInterest() { interests.Clear(); }

    public void ToggleBelgiumNews() {

        if(isBelgiumNewsWanted) isBelgiumNewsWanted = false;
        else isBelgiumNewsWanted = true;
    
    }

    public void ToggleWorldNews() {
        
        if(isWorldNewsWanted) isWorldNewsWanted = false;
        else isWorldNewsWanted = true;

    }

    public bool HasNotInterest() { return interests.Capacity == 0; }

}