/************************************************************************************************************************************************************************
 * Script attached to the Attack Scene Handler game object 
 * Used to set up and control the attack scene 
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

public class AttackScene : MonoBehaviour
{
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

    public GameObject towerPrefab;
    // Turret game objects
    public GameObject turr1;
    public GameObject turr2;
    public GameObject turr3;

    private List<int> turretTypes;

    private Transform ground;
    private bool loadDone;
    private List<Vector3> positions;

    public GameObject buildPlaces;
    public GameObject loadingPanel;
    public GameObject gameOverPanel;
    public GameObject player;
    public GameObject WinPanel;
    public GameObject villageHub;

    //Shop and mmoney stuff
    public static int userBalance;
    private bool init;
    private string userID;
    private bool loadDone2;
    private bool onlyOnce;
    ShowToast toastMaker;
    public ShowToast WinToastMaker;
    public ShowToast LoseToastMaker;
    public ShowToast ShopToastMaker;
    bool balChangedOnWinOrLose;

    //Shop stuff
    public GameObject zombie1;
    public GameObject zombie2;
    public GameObject zombie3;

    //Player Health stuff
    public Text health;
    public GameObject playerHealth;

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

    //Used to initialize the scene. Each block of code has individual comments
    void Start()
    {
        //toastMaker = ShowToast.instance;
        balChangedOnWinOrLose = false;

        gameOverPanel.SetActive(false);
        WinPanel.SetActive(false);
        loadingPanel.SetActive(true);
        InitializeFirebase();

        //Removes the buildplace script from each buildplace tile so attackers cant place turrets 
        foreach (Transform child in buildPlaces.transform)
        {
            Destroy(child.gameObject.GetComponent<Buildplace>());
        }

        GameObject temp = GameObject.Find("Ground (1)");
        ground = temp.GetComponent<Transform>();

        email = user.Email;
        //Debug.Log(email);
        userNameText.text = email;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://finalprojectunity-31171.firebaseio.com//");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        //Gets the id for the village in order to populate the map with the correct turrets 
        vilRef = reference.Child("villages");
        villageID = Scenes.getParam("villageID");

        positions = new List<Vector3>();
        turretTypes = new List<int>();

        //Retrieve turrets from the village 
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
                    Debug.Log(s.Value.ToString());
                    Debug.Log(villageID);
                    Debug.Log((s.Child("villageID").Value.ToString()));
                    if (villageID.Equals(s.Child("villageID").Value.ToString()))
                    {

                        //If statement to get type to choose which prefab
                        positions.Add(new Vector3(float.Parse(s.Child("position").Child("x").Value.ToString()), float.Parse(s.Child("position").Child("y").Value.ToString()), float.Parse(s.Child("position").Child("z").Value.ToString())));

                        turretTypes.Add(int.Parse(s.Child("type").Value.ToString()));
                    }
                }
                loadDone = true;
                Debug.Log("Done loading turrets");
            }
        });

        //Finds the user id for the logged in user in order to update their balance when the player exits 
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

    public void backButton()
    {
        Application.Quit();
    }

    public bool playerKilled;


    //Whent he return button is pressed, the balance of the player is udated on the database and the world scene is loaded
    public void returnButton()
    {
        reference.Child("users").Child(userID).Child("balance").SetValueAsync(userBalance);

        SceneManager.LoadSceneAsync(1);
        //SceneManager.LoadScene(1);
    }

    //Used to update the attack scene. each block of code within has been commented 
    private void Update()
    {
        //Updates the health seen in the ui according to the players actual health
        if (playerHealth != null)
        {
            health.text = playerHealth.GetComponent<TextMesh>().text;
        }
        else {
            health.text = "";
        }
        
        //Gets the users balance from the database at the start of the scene and stores in static variable (userBalance)
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
                        if (!onlyOnce)
                        {
                            userBalance = int.Parse(s.Child("balance").Value.ToString());
                            onlyOnce = true;
                        }
                    }
                }
            }
        });

        /* If the balance has been retrieved from the database then 
        * it uses the static variable to update the text representing the players balance on the ui */
        if (onlyOnce)
        {
            balanceText.text = "£" + userBalance.ToString();
        }

        //Once the userID and villages turrets have been retreived from database the village is populated and the load panel is hidden
        if (loadDone && loadDone2)
        {
            for (int i = 0; i < positions.Count; i++)
            {
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

        //If village hub is destroyed then show the win panel else if the player is destroyed show the lose panel 
        if (villageHub == null) {
            WinPanel.SetActive(true);

            //Stop player moving after killing village by disabling script
            MovementTest mTest = player.GetComponent<MovementTest>();
            mTest.Stop();
            if (!balChangedOnWinOrLose)
            {
                userBalance = userBalance + 500;
                balChangedOnWinOrLose = true;
                //Show Toast
                WinToastMaker.showToast("Congratulations! You have earned £500", 2);
            }
        } else if (player == null) {
            gameOverPanel.SetActive(true);
            if (!balChangedOnWinOrLose)
            {
                userBalance = userBalance - 500;
                balChangedOnWinOrLose = true;
                //Show Toast
                //toastMaker.setTextObj(LoseToastText);
                LoseToastMaker.showToast("You have failed to take over this village. Forced to pay out £500", 2);
            }
        }
    }


    //Methods called upon pressing the attack scene shop (the  zombie shop)
    //If balance of player is above the cost then the zombie prefab is spawned on the location of the player and a message is shown
    //Else a message stating the player cannot afford to purcahse 
    public void zombie1Pressed()
    {
        // TODO build stuff...
        //Make sure not on top of UI 

        int balance = userBalance;
        int cost = 100;

        if (cost > balance) {
            //Show Toast saying cannot afford
            ShopToastMaker.showToast("You can not afford to purchase Zombie Pack 1 for £100", 2);
        } else {

            balance = balance - cost;
            userBalance = balance;
            //Show Toast saying spent
            GameObject zombiesToSpawn = zombie1;
            GameObject g = (GameObject)Instantiate(zombiesToSpawn, player.transform.position, player.transform.rotation);
            g.transform.SetParent(ground);
            ShopToastMaker.showToast("You have purchased Zombie Pack 1 for £100", 2);
        }
    }

    public void zombie2Pressed()
    {
        // TODO build stuff...
        //Make sure not on top of UI 

        int balance = userBalance;
        int cost = 200;

        if (cost > balance)
        {
            //Show Toast saying cannot afford
            ShopToastMaker.showToast("You can not afford to purchase Zombie Pack 2 for £200", 2);
        }
        else
        {

            balance = balance - cost;
            userBalance = balance;
            //Show Toast saying spent
            GameObject zombiesToSpawn = zombie2;
            GameObject g = (GameObject)Instantiate(zombiesToSpawn, player.transform.position, player.transform.rotation);
            g.transform.SetParent(ground);

            //Moves the other zombies out of being child objects of first zombie as destroying first zombie 
            //Destroys child 
            foreach (Transform child in g.transform)
            {
                Debug.Log("CHILD ZOMBIE");
                if (child.tag == "Player")
                    child.parent = ground;
            }

            ShopToastMaker.showToast("You have purchased Zombie Pack 2 for £200", 2);
        }
    }

    public void zombie3Pressed()
    {
        // TODO build stuff...
        //Make sure not on top of UI 

        int balance = userBalance;
        int cost = 300;

        if (cost > balance)
        {
            //Show Toast saying cannot afford
            ShopToastMaker.showToast("You can not afford to purchase Zombie Pack 3 for £300", 2);
        }
        else
        {

            balance = balance - cost;
            userBalance = balance;
            //Show Toast saying spent
            GameObject zombiesToSpawn = zombie3;
            GameObject g = (GameObject)Instantiate(zombiesToSpawn, player.transform.position, player.transform.rotation);
            g.transform.SetParent(ground);

            //Moves the other zombies out of being child objects of first zombie as destroying first zombie 
            //Destroys child 
            foreach (Transform child in g.transform)
            {
                Debug.Log("CHILD ZOMBIE");
                if (child.tag == "Player")
                    child.parent = ground;
            }

            foreach (Transform child in g.transform)
            {
                Debug.Log("CHILD ZOMBIE");
                if (child.tag == "Player")
                    child.parent = ground;
            }


            ShopToastMaker.showToast("You have purchased Zombie Pack 3 for £300", 2);
        }
    }
}
