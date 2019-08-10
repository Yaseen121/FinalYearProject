// Script attached to the player object when attacking a village
// Used to Check when player touches the enemy village and upon doing so it triggers the decraseAll method on its health effectively killing it
// Class created by: Mohammed Yaseen Sultan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackingPlayer : MonoBehaviour
{
    //int hits = 0;

    //If player touches the enemy village hub then decrease all its health
    void OnTriggerEnter(Collider collision)
    {
        // If castle then deal Damage
        if (collision.name == "VillageHub")
        {
            collision.GetComponentInChildren<Health>().decreaseAll();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If castle then deal Damage
        if (collision.gameObject.name == "VillageHub")
        {
            collision.gameObject.GetComponentInChildren<Health>().decreaseAll();
        }
    }

}
