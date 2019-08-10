/************************************************************************************************************************************************************************
 * Script attached to players, zombies and village hub
 * Used to manage the health of the attached entity/game object
 * Script taken from Tower Defense Tutorial (Authors name not published on website) 
 * The turoial followed can be found here: https://noobtuts.com/unity/tower-defense-game
 ************************************************************************************************************************************************************************/

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    // The TextMesh Component
    TextMesh tm;

    // Use this for initialization
    void Start () {
        tm = GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update () {
        // Face the Camera
        //transform.forward = Camera.main.transform.forward;
    }

    // Return the current Health by counting the '-'
    public int current() {
        return tm.text.Length;
    }

    // Decrease the current Health by removing one '-'
    public void decrease() {
        if (current() > 1) {
            //tm.text = tm.text.Remove(tm.text.Length - 1);
            tm.text = tm.text.Substring(0, tm.text.Length - 1);
            Debug.Log(tm.text);
        } else  {
            Destroy(transform.parent.gameObject);
        }
    }

    //Destroy all health (Player attack to village hub)
    public void decreaseAll()
    {
        Destroy(transform.parent.gameObject);
    }
}
