/************************************************************************************************************************************************************************
 * Static script used to hold the currently selected turret 
 * so that players can repeatdely press on locations without having to reselect a turret on the shop
 * Script based of YouTube tutorial by: Brackeys  
 * The tutorial can be found here: https://www.youtube.com/watch?v=uv1zp7aOoOs
 ************************************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{

    //Singleton pattern 
    public static BuildManager instance;
    
    private void Awake()
    {
        if (instance != null) {
            Debug.Log("More than one build manager exists");
            return;
        }
        instance = this;
    }

    //Base/Default turret
    public GameObject turret1Prefab;
    //Other turrets
    public GameObject turret2Prefab;
    public GameObject turret3Prefab;

    //Get selected turret type id
    public int instanceType;
    public int cost;

    public ShowToast BuildShopToast;

    public int getTurretType() {
        return instanceType;
    }

    private void Start()
    {
        //Default to first turret
        turretToBuild = turret1Prefab;
        instanceType = 1;
        cost = 50;
    }

    private GameObject turretToBuild;

    //Getters and Setters
    public GameObject GetTurretToBuild() {
        return turretToBuild;
    }

    public int getCost() {
        return cost;
    }

    //called from shop
    public void setTurret1()
    {
        turretToBuild = turret1Prefab;
        instanceType = 1;
        cost = 50;
    }

    public void setTurret2()
    {
        turretToBuild = turret2Prefab;
        instanceType = 2;
        cost = 100;
    }

    public void setTurret3()
    {
        turretToBuild = turret3Prefab;
        instanceType = 3;
        cost = 200;
    }
}
