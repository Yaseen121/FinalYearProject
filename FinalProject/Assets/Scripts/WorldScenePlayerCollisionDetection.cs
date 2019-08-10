// Script attached to the player avatar on the world scene
// This used is to detect when the user is within the radius of another village  
// Script written by: Mohammed Yaseen Sultan


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScenePlayerCollisionDetection : MonoBehaviour
{
    //Static bool to get the state of player (collding with village or not)
    public static bool getColliding;
    //Static reference to the script itself so it can be accessed by other scripts
    public static WorldScenePlayerCollisionDetection instance;

    //Ensures there is only one instance of this script
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one build manager exists");
            return;
        }
        instance = this;
    }

    //Initializes bool 
    private void Start()
    {
        getColliding = false;     
    }

    //Methods that updaate the bool 
    public bool isCollidingNow()
    {
        return getColliding;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Village"))
        {
            getColliding = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Village"))
        {
            getColliding = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Village"))
        {
            getColliding = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Village"))
        {
            getColliding = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Village"))
        {
            getColliding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Village"))
        {
            getColliding = false;
        }
    }


}
