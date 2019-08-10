// Class to represent each player account on the firebase database
// Holds the users email, the player id, their balance (money) and their village id (set to -1 if they do not have a village)
// Villages are held in a different database table and so the village id is used to link players with villages
// Class created by: Mohammed Yaseen Sultan

using System;

internal class Accounts
{

    //Fields stored in the database for each account 
    public String email;
    public int balance;
    public String id;
    public String villageId;

    //Constructors used to creat instances of accounts which are used to upload to database 
    public Accounts(String e, String id)
    {
        this.email = e;
        this.id = id;
        this.balance = 1000;
        this.villageId = "-1";
    }

    public Accounts(String e, String id, int balance, String vId)
    {
        this.email = e;
        this.id = id;
        this.balance = balance;
        this.villageId = vId;
    }

    //Getters and setters
    public String getEmail()
    {
        return email;
    }

    public String getId()
    {
        return id;
    }

    public double getBalance()
    {
        return balance;
    }

    public String getVillageId() {
        return villageId;
    }

    public void spent(int x)
    {
        this.balance = this.balance - x;
    }

    public void earned(int x)
    {
        this.balance = this.balance + x;
    }

    public void setVillageId(String vill) {
        this.villageId = vill;
    }
}
