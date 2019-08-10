/************************************************************************************************************************************************************************
 * Script attached to each build place/tile 
 * Used to place spawn turrets on locations 
 * Script based on a Tower Defence tutorial (No author name published on website) and Brackeys from YouTube 
 * The tower defece  turoial followed can be found here: https://noobtuts.com/unity/tower-defense-game
 * The Brackeys video used can be found here: https://www.youtube.com/watch?v=t7GuWvP_IEQ&t=2s
 ************************************************************************************************************************************************************************/

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Buildplace : MonoBehaviour {

    // The Tower that should be built
    // public GameObject towerPrefab;
    //public GameObject turret1Prefab;
    private Transform ground;
    private string vilID;

    private DatabaseReference reference;
    private DatabaseReference turretRef;

    private GameObject turret;
    private int balance;

    
    public ShowToast BuildSceneToast;

    //Firebase databse reference 
    private void Start() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://finalprojectunity-31171.firebaseio.com//");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        
        ////Get email

        GameObject temp = GameObject.Find("Ground (1)");
        ground = temp.GetComponent<Transform>();
        vilID = Scenes.getParam("villageID");
        turretRef = reference.Child("turrets");
    }

    //When buildplace object pressed spawn the selected turret if user has enough money and there is no turrent existeing there already 
    void OnMouseUpAsButton() {
        // TODO build stuff...
        //Make sure not on top of UI 


        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        if (turret != null) {
            Debug.Log("Cant Build Here");
            //
            BuildManager.instance.BuildShopToast.showToast("You cannot build here as there is already a turret", 2);
        } else {
            balance = BuildAndEditScene.userBalance;
            int type = BuildManager.instance.getTurretType();
            int cost = BuildManager.instance.getCost();
            if (cost > balance)
            {
                Debug.Log(balance);
                Debug.Log("You can't afford this turret");
                //
                BuildManager.instance.BuildShopToast.showToast("You cannot afford to purchas this turret for £" + cost, 2);
            }
            else {
                balance = balance - cost;
                BuildAndEditScene.userBalance = balance;
                Debug.Log(BuildAndEditScene.userBalance);
                //Brackeys Code slightly edited -----> Reference this
                GameObject turretToBuild = BuildManager.instance.GetTurretToBuild();
                GameObject g = (GameObject)Instantiate(turretToBuild, transform.position + (5 * Vector3.up), transform.rotation);
                g.transform.SetParent(ground);
                turret = g;
                //int type = BuildManager.instance.getTurretType();
                Debug.Log("Current Village ID " + vilID);

                //Uploads turret data to database 
                string turretID = turretRef.Push().Key;
              
                Turret tur = new Turret(type, g.transform.position, vilID, turretID);
                string json = JsonUtility.ToJson(tur);
                turretRef.Child(turretID).SetRawJsonValueAsync(json);
                //
                BuildManager.instance.BuildShopToast.showToast("You have purchased a turret for £" + cost, 2);
            }
        }
    }


    private bool TurretFound = false ; 

    //Checks if turrent is already on the build place game object
    void FixedUpdate()
    {
        if (!TurretFound)
        {
            Vector3 up = transform.TransformDirection(Vector3.up);
            //(Physics.Raycast(gameObject.transform.position, up, 5)
            RaycastHit hitInfo;
            if (Physics.Raycast(gameObject.transform.position, up, out hitInfo, 5)) {
                print("There is something above of the object!");
                TurretFound = true;
                //Destroy(gameObject);
                turret = hitInfo.transform.gameObject;
            }
                
        }
    }
}