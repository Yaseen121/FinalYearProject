// Class to represent each village on the firebase database 
// Holds the users email, the village id, the size of the village and the latitude and longtitude co ordinates of the village 
// Class created by: Mohammed Yaseen Sultan

using System;
using System.Collections.Generic;

public class Villages {

    //Fields stored in the database for each village 
    public String email;
    public Double lat;
    public Double lng;
    public string id;
    public int size;

    //Constructors used to creat instances of villages which are used to upload to database 
    public Villages(String e, Double lat, Double lng, string id) {
        this.email = e;
        this.lat = lat;
        this.lng = lng;
        this.id = id;
        this.size = 1;
    }

    public Villages(String e, String lat, String lng,  string id) {
        this.email = e;
        this.lat = Convert.ToDouble(lat);
        this.lng = Convert.ToDouble(lng);
        this.id = id;
        this.size = 1;
    }

    public Villages(String e, String lat, String lng, string id, String size)
    {
        this.email = e;
        this.lat = Convert.ToDouble(lat);
        this.lng = Convert.ToDouble(lng);
        this.id = id;
        this.size = Convert.ToInt32(size);
    }
    
    //Getters 
    public int getSize() {
        return size;
    }

    public string getEmail()
    {
        return email;
    }
    public string getVillageID() {
        return id;
    }

    public double getLat() {
        return lat;
    }

    public double getLng() {
        return lng;
    }
}
