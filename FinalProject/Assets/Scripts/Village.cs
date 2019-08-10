// Script attached the 3D models that represent village in the world sccene 
// Used to load the correct scene on press 
// Script created by: Mohammed Yaseen Sultan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Village : MonoBehaviour
{
    public string villageID;
    public string ownerEmail;

    public Color red;
    public Color green;
    public Renderer rend;

    private Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseUser user;
    private string email;

    public ShowToast WorldSceneToastMaker;

    public void setID(string oE, string vID) {
        ownerEmail = oE;
        villageID = vID;
    }

    //Check if its the logged in users village if it is it loads the edit scene 
    //Otherwise it loads the attack scene if the user has £500 or more 
    private void OnMouseDown()
    {
        if (ownerEmail.Equals(email))
        {
            Debug.Log("This is your village, you may edit it");
            //Scenes.Load("BuildScene", "villageID", villageID);
            Scenes.Load("BuildScene - Copy", "villageID", villageID);
            //SceneManager.LoadScene(2);
        }
        else {
            //Check balance here
            int money = WorldScene.curBal;
            Debug.Log("current balance of attacking player " + money);

            if (money >= 500)
            {
                Debug.Log("This is not your village, you may attack it");
                //Switch to attack scene
                //Scenes.Load("AttackScene", "villageID", villageID);

                Scenes.Load("AttackScene - Copy - Copy (2)", "villageID", villageID);
            }
            else {
                WorldSceneToastMaker.showToast("You do not have enough money to attack this village", 2);
            }

        }
        
    }

    //Gets the renderer for each village and the ToastText of the scene upon initialization 
    private void Start()
    {
        WorldSceneToastMaker.toasTxt = GameObject.Find("ToastText").GetComponent<Text>();
        rend = GetComponent<Renderer>();
        InitializeFirebase();
        email = user.Email;
    }

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

}
