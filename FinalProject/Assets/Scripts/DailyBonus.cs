// Class to manage daily bonus for each player saved on the firebase database 
// Holds the email of the user, and the time of last login
// Used when creating a player and each time a player logs in (each time world scene is loaded) 
// Class created by: Mohammed Yaseen Sultan

using System;

internal class DailyBonus
{
    //Fields stored in the database for each daily bonus for player 
    public String email;
    public String lastBonusTime;
    public String id;

    //Constructors used to creat instances of villages which are used to upload to database 
    public DailyBonus(String e, string id)
    {
        this.email = e;
        this.lastBonusTime = DateTime.Now.ToString();
        this.id = id;
    }

    //Getters and setters 
    public String getTime() {
        return lastBonusTime;
    }

    public void setTime(DateTime newTime) {
        this.lastBonusTime = newTime.ToString();
    }
}
