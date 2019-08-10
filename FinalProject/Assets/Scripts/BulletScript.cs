/************************************************************************************************************************************************************************
 * Script attached to the bullets
 * Used to move bullet and damage target upon hit
 * Script written by : "Brackeys" on YouTube
 * The turoial followed can be found here: https://www.youtube.com/watch?v=oqidgRQAMB8&t=1s
 ************************************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Transform target;

    public float speed = 20f;
    public int damage = 1;

    public void Seek(Transform _target) {
        target = _target;
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        if (dir.magnitude <= distanceThisFrame) {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    public void HitTarget() {
        Debug.Log("Target HIT!");

        Health health = target.GetComponentInChildren<Health>();
        if (health) {
            for (int i = 0; i < damage; i++) {
                health.decrease();
            }
            Destroy(gameObject);
        }
    }
}
