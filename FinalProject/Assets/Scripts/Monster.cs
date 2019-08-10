/************************************************************************************************************************************************************************
 * Script attached to the zombies/AI used to help while player is attacking
 * Used to make monster target the village hub and deal damage upon hitting it 
 * Script taken from Tower Defense Tutorial (Authors name not published on website) 
 * The turoial followed can be found here: https://noobtuts.com/unity/tower-defense-game
 ************************************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour {

    // Navigate to Castle
    GameObject villageHub;
    int hits = 0;

    // Use this for initialization
    void Start () {
        villageHub = GameObject.Find("VillageHub");
        if (villageHub)
            GetComponent<NavMeshAgent>().destination = villageHub.transform.position;
    }

    void OnTriggerEnter(Collider co){
        // If castle then deal Damage
        if (co.name == "VillageHub") {
            co.GetComponentInChildren<Health>().decrease();
            Destroy(gameObject);
        }
    }

}
