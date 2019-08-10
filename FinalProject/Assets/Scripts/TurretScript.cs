/************************************************************************************************************************************************************************
 * Script attached to the turrets 
 * Used to track the nearest enemy to the turret and spawn bullets 
 * Script written by : "Brackeys" on YouTube
 * The turoial followed can be found here: https://www.youtube.com/watch?v=QKhn2kl9_8I 
 ************************************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    private Transform target;

    [Header("Turret Attributes")]
    public float range = 15;
    public float turnSpeed = 5f;
    public float fireRate = 1f;
    public float fireCountDown = 0f;

    [Header("Unity Setup")]
    public Transform partToRotate;
    public GameObject bullterPrefab;
    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) {
            return;
        }

        Vector3 dir = target.position - transform.position;
        Quaternion lookRation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        //Controls rate of fire 
        if (fireCountDown <= 0f) {
            Shoot();
            fireCountDown = 12f / fireRate;
        }
        fireCountDown -= Time.deltaTime;
    }

    //Spawns a bullet and targets closet enemy 
    void Shoot() {
        //Debug.Log("SHOOT!");
        GameObject bulletGO = (GameObject) Instantiate(bullterPrefab, firePoint.position, firePoint.rotation);
        BulletScript bullet = bulletGO.GetComponent<BulletScript>();
        if (bullet != null) {
            bullet.Seek(target);
        }
        //Play sound for bullet here
        gameObject.GetComponent<AudioSource>().Play();
    }

    //Finds closest target within range 
    void UpdateTarget() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else {
            target = null;
        }
    }

    //Shows range of turrets in the editor 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
