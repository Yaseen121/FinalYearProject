// Class to represent each turret 
// Holds the type of turret, the position of the turret in the scene, the village id it belongs to and the id of the turret
// Class created by: Mohammed Yaseen Sultan

 using UnityEngine;

public class Turret
{
    //Fields stored in the database for each village 
    public int type;
    public Vector3 position;
    public string villageID;
    public string turretID;

    //Constructors used to creat instances of turret which are used to upload to database 
    public Turret(int t, Vector3 p, string vID, string tID) {
        this.type = t;
        this.position = p;
        this.villageID = vID;
        this.turretID = tID;
    }

    //Getters
    public int getType()
    {
        return type;
    }

    public Vector3 getTransform() {
        return position;
    }

    public string getVillageID() {
        return villageID;
    }

    public string getTurretID()
    {
        return turretID;
    }
}