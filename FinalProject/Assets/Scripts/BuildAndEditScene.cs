/************************************************************************************************************************************************************************
 * Script attached to the Build and Edit Scene Handler game object 
 * Used to control the loading of existing turrets and updating the balance of the user when they buy new turrets for their village
 * Three methods have been used from the firebase documentation 
 * This documentation can be found here: https://firebase.google.com/docs/auth/unity/start 
 * All other ccode has been written by: Mohammed Yaseen Sultan
 ************************************************************************************************************************************************************************/


using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuildAndEditScene : MonoBehaviour {
    //UI Stuff
    public Text userNameText;
    public Text balanceText;

    private DatabaseReference reference;
    private DatabaseReference vilRef;
    private Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseUser user;
    private string email;
    private string bal = "£";
    private string villageID;

    public GameObject loadingPanel;

    public GameObject towerPrefab;
    // Turret game objects
    public GameObject turr1;
    public GameObject turr2;
    public GameObject turr3;

    private Transform ground;
    private bool loadDone;
    private List<Vector3> positions;

    private List<int> turretTypes;

    public static int userBalance;
    private bool init;

    private string userID;
    private bool loadDone2;
    private bool onlyOnce;

    // Use this for initialization firebase methods 

    // Handle initialization of the necessary firebase modules:
    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }
    

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    //Activates loading panel as turrets are being loaded in from database into list to be rendered on screen
    //Also retrieves the logged in user ID for database (To update balance upon exitting sccene) 
    void Start()
    {
        loadingPanel.SetActive(true);
        InitializeFirebase();
        GameObject temp = GameObject.Find("Ground (1)");
        ground = temp.GetComponent<Transform>();
  
        email = user.Email;
        
        //Debug.Log(email);
        userNameText.text = email;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://finalprojectunity-31171.firebaseio.com//");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        vilRef = reference.Child("villages");
        villageID = Scenes.getParam("villageID");


        positions = new List<Vector3>();
        turretTypes = new List<int>();
        FirebaseDatabase.DefaultInstance.GetReference("turrets").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Some Error");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("What happening");
                DataSnapshot dataSnapshot = task.Result;
                // Do something with snapshot...
                foreach (DataSnapshot s in dataSnapshot.Children)
                {
                    //Debug.Log(s.Value.ToString());
                    //Debug.Log(villageID);
                    //Debug.Log((s.Child("villageID").Value.ToString()));
                    if (villageID.Equals(s.Child("villageID").Value.ToString()))
                    {

                        //If statement to get type to choose which prefab
                        positions.Add(new Vector3(float.Parse(s.Child("position").Child("x").Value.ToString()), float.Parse(s.Child("position").Child("y").Value.ToString()), float.Parse(s.Child("position").Child("z").Value.ToString())));

                        Debug.Log("Turret type :" + s.Child("type").Value.ToString());
                        turretTypes.Add(int.Parse(s.Child("type").Value.ToString()));


                    }
                }
                loadDone = true;
                Debug.Log("Done loading turrets");
            }
        });

        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Some Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                // Do something with snapshot...
                foreach (DataSnapshot s in dataSnapshot.Children)
                {
                    //Debug.Log(s.Value.ToString());
                    //Debug.Log(villageID);
                    //Debug.Log((s.Child("villageID").Value.ToString()));
                    if (email.Equals(s.Child("email").Value.ToString()))
                    {

                        userID = s.Child("id").Value.ToString();
                        loadDone2 = true;
                        Debug.Log("Done loading userID");
                        break;
                    }
                }
                
               
            }
        });
    }

    //Updates the balance of the user and updates the size of the village according to the number 
    //of turrets within it and then loads the world scene
    public void returnButton() {
        GameObject[] Turret = GameObject.FindGameObjectsWithTag("Turret");
        Debug.Log(Turret.Length);
        reference.Child("villages").Child(villageID).Child("size").SetValueAsync(Turret.Length -1);
        reference.Child("users").Child(userID).Child("balance").SetValueAsync(userBalance);


        SceneManager.LoadSceneAsync(1);
        //SceneManager.LoadScene(1);
    }

    //Gets the balance of the user from databse and stores in static varialbe (userBalance)
    //Uses static variable to update the UI that represents the balance of the user 
    //Renders/instantiates the Turrets once they have finished loading and then hides the loading panel once done
    private void Update()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                // Do something with snapshot...
                foreach (DataSnapshot s in dataSnapshot.Children)
                {
                    if (email.Equals(s.Child("email").Value.ToString()))
                    {
                        //Debug.Log("LOOK HERE " + s.Child("balance").Value.ToString());
                        bal = "£" + s.Child("balance").Value.ToString();
                        if (!onlyOnce) {
                            userBalance = int.Parse(s.Child("balance").Value.ToString());
                            onlyOnce = true;
                        }
                    }
                }
            }
        });
        //balanceText.text = bal;
        Debug.Log("UserBalance : " + userBalance);
        if (onlyOnce) {
            balanceText.text = "£" + userBalance.ToString();
        }

        

        if (loadDone && loadDone2) {
            for (int i = 0; i < positions.Count; i++) {
                //////
                ///new stuff
                GameObject g;
                if (turretTypes[i] == 1)
                {
                    g = (GameObject)Instantiate(turr1);
                }
                else if (turretTypes[i] == 2)
                {
                    g = (GameObject)Instantiate(turr2);
                }
                else
                {
                    g = (GameObject)Instantiate(turr3);
                }

                g.transform.position = positions[i];
                g.transform.SetParent(ground);
            }
            loadingPanel.SetActive(false);
            loadDone = false;
        }
    }
}
