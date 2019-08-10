/************************************************************************************************************************************************************************
 * Script attached to the world scene Handler game object 
 * Three methods have been used from the firebase documentation 
 * This documentation can be found here: https://firebase.google.com/docs/auth/unity/start 
 * All other ccode has been written by: Mohammed Yaseen Sultan
 ************************************************************************************************************************************************************************/

 
 using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldScene : MonoBehaviour {
    public Text userNameText;
    public Text balanceText;
    public Text location;
    public Button makeVillage;

    private DatabaseReference reference;
    private DatabaseReference vilRef;
    private Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseUser user;
    private string email;
    private string bal = "£";
    private bool alreadyHasVil;

    public WorldScenePlayerCollisionDetection PlayerCheck;
    public ShowToast WorldSceneToastMaker;

    public static int curBal; 


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
                //Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                //Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    private bool BonusChecked = false;
    private bool bonus = false;
    private string BonusTime;

    //Initializes the scene and and checks if player is eligble for daily bonus
    //Checks if the player already has a village as well 
    void Start () {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://finalprojectunity-31171.firebaseio.com//");
        PlayerCheck = WorldScenePlayerCollisionDetection.instance;
        makeVillage.gameObject.SetActive(false);
        helpPanel.SetActive(false);
        InitializeFirebase();
        alreadyHasVil = false;
        Debug.Log("This ");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        email = user.Email;
        //Debug.Log(email);
        userNameText.text = email; 
        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://finalprojectunity-31171.firebaseio.com//");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        vilRef = reference.Child("villages");
        FirebaseDatabase.DefaultInstance.GetReference("villages").GetValueAsync().ContinueWith(task => {
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
                    if (email.Equals(s.Child("email").Value.ToString())) {
                        alreadyHasVil = true;
                    }
                }
            }
        });

        BonusChecked = false;
        
        FirebaseDatabase.DefaultInstance.GetReference("DailyBonus").GetValueAsync().ContinueWith(task => {
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
                        DateTime lastLoggedIn = DateTime.Parse(s.Child("lastBonusTime").Value.ToString());
                        DateTime minForBonus = lastLoggedIn.AddDays(1);
                        TimeSpan timeNeeded = minForBonus - DateTime.Now;
                        if (timeNeeded < TimeSpan.Zero)
                        {
                            Debug.Log("BONUS AVAILABLE");
                            //Add money to account
                            bonus = true;
                            //Change last bonus receibed time to now
                            reference.Child("DailyBonus").Child(s.Child("id").Value.ToString()).Child("lastBonusTime").SetValueAsync(DateTime.Now.ToString());
                            
                        }
                        else {
                            Debug.Log("TIME NEEDED: " + timeNeeded);
                            BonusTime = "Daily Login bonus availabe in : " + timeNeeded.Hours + " Hours and " + timeNeeded.Minutes + " Minutes";
                        }
                        BonusChecked = true;
                    }
                }
            }
        });

    }

    //Method to create a village for the player, this is attached a button that is only available if the player does not have a village 
    //Once village is created (and database updated) the edit village scene is loaded
    public void CreateVillage() {
        if (PlayerCheck.isCollidingNow()) {
            //Show Toast
            WorldSceneToastMaker.showToast("You cannot build here as you are in an enemy zone", 2);
            return;
        }
        String id = vilRef.Push().Key;
        //Debug.Log(reference);
        //Debug.Log(reference.Child("villages"));
        //Debug.Log(id);
        String[] loc = location.text.Split(',');//lng then lat
        double lat = Convert.ToDouble(loc[1]);
        double lng = Convert.ToDouble(loc[0]);
        Villages vil = new Villages(user.Email, lat, lng, id);
        //reference.Child("villages").Child(key).Child("User").SetValueAsync(email);
        //vilRef.Child(id).Child("User").SetValueAsync(email);

        /*
         *  string json = JsonUtility.ToJson(user);

        mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
        */
        String json = JsonUtility.ToJson(vil);
        vilRef.Child(id).SetRawJsonValueAsync(json);


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
                    if (user.Email.Equals(s.Child("email").Value.ToString())) {
                        //Debug.Log("Here: " + s.Child("email").Value + " " + s.Child("id").Value + " " + s.Child("villageID").Value + " " + s.Child("balance").Value);
                        //Villages vil = new Villages(s.Child("email").Value.ToString(), s.Child("lat").Value.ToString(), s.Child("lng").Value.ToString());
                        Accounts acc = new Accounts(s.Child("email").Value.ToString(), s.Child("id").Value.ToString(), int.Parse(s.Child("balance").Value.ToString()), id);
                        json = JsonUtility.ToJson(acc);
                        reference.Child("users").Child(s.Child("id").Value.ToString()).SetRawJsonValueAsync(json);
                    }
                }
            }
        });

        //Change scene
        //SceneManager.LoadSceneAsync(2);
        Scenes.Load("BuildScene - Copy", "villageID", id);
    }

    //Deactivates the create village button if the player already has a village. 
    //Displays a message/toast saying that a login bonus was recieved or gives the time to next login bonus avaialbe 
    //If bonus recieved updates balance on database
    void Update () {
        if (!alreadyHasVil)
        {
           // Debug.Log("MAKE THIS ACTIVE");
            makeVillage.gameObject.SetActive(true);
        }
        else {
            //Debug.Log("MAKE THIS INACTIVE");
            makeVillage.gameObject.SetActive(false);
        }
        FirebaseDatabase.DefaultInstance.GetReference("villages").GetValueAsync().ContinueWith(task => {
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
                        alreadyHasVil = true;
                    }
                }
                //Debug.Log("Checked if owns Village");
            }
        });
        if (BonusChecked) {
            if (bonus)
            {
                WorldSceneToastMaker.showToast("Daily login bonus recieved of £500", 2);
            }
            else {
                WorldSceneToastMaker.showToast(BonusTime, 2);
                
            }
            BonusChecked = false;
        }
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
                        curBal = int.Parse(s.Child("balance").Value.ToString());
                        Debug.Log("Current balancce is " + curBal);
                        if (bonus)
                        {
                            reference.Child("users").Child(s.Child("id").Value.ToString()).Child("balance").SetValueAsync(curBal + 500);
                            bonus = false;
                        }
                        bal = "£" + s.Child("balance").Value.ToString();
                        //Debug.Log("Got Balance");
                        break;  
                    }
                }
            }
        });
        balanceText.text = bal;
    }

    //Signs out the user on firebase and loads Home scene 
    public void LogOut() {
        auth.SignOut();
        SceneManager.LoadScene(0);
    }


    //Help Panel/Tutorial Stuff
    public GameObject helpPanel;

    //Opens and closes help panel methods 
    public void helpPressed() {
        helpPanel.SetActive(true);
    }

    public void helpClosed()
    {
        helpPanel.SetActive(false);
    }
}
