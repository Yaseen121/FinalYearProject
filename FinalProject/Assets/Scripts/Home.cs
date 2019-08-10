/************************************************************************************************************************************************************************
 * Script attached to the Home Scene Handler game object 
 * Used for creating users on the firebase database, authenticating users and loading the world scene 
 * Most of the code has been taking from the firebase documentation 
 * This documentation can be found here: https://firebase.google.com/docs/auth/unity/start 
 * All other code has been written by: Mohammed Yaseen Sultan
 ************************************************************************************************************************************************************************/

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Home : MonoBehaviour {
    private DatabaseReference reference;
    private DatabaseReference accountRef;

    protected Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseAuth otherAuth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();
    private string logText = "";
    public Text emailText;
    public Text passwordText;
    protected string email = "";
    protected string password = "";
    protected string displayName = "";
    private bool fetchingToken = false;
    const int kMaxLogSize = 16382;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    public GameObject MainPanel;
    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public GameObject SharedFieldsPanel;

    public GameObject AttemptingLoginPanel;
    private bool loginFin;

    public GameObject AttemptingRegPanel;
    private bool regFin;
    private bool regSuccess;

    public ShowToast toastMaker;

    /// Panel Management methods ///

    //The scene is split into 3 panels, main menu, login and regster
    //This methods open the corresponding panels 
    public void ReturnToMainMenu() {
        MainPanel.SetActive(true);
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        SharedFieldsPanel.SetActive(false);
    }

    public void OpenLoginMenu()
    {

        //Debug.Log("TESTING 123 LOGIN");
        MainPanel.SetActive(false);
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
        SharedFieldsPanel.SetActive(true);
    }

    public void OpenRegisterMenu()
    {
        MainPanel.SetActive(false);
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
        SharedFieldsPanel.SetActive(true);
    }

    //Closes the game
    public void QuitPressed()
    {
        Application.Quit();
    }
 



    // Use this for initialization
    void Start () {
        //Set the main menu panel to acctive upon starting the game 
        MainPanel.SetActive(true);
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        SharedFieldsPanel.SetActive(false);
        AttemptingLoginPanel.SetActive(false);
        loginFin = false;
        AttemptingRegPanel.SetActive(false);
        regFin = false;

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://finalprojectunity-31171.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        accountRef = reference.Child("users");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    // Update is called once per frame
    // Exit if escape (or back, on mobile) is pressed.
    //If user login authenticated then world scene is loaded 
    //If user registration is succeessful opens the login panel 
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (emailText != null)
        {
            email = emailText.text;
        }
        if (passwordText != null)
        {
            password = passwordText.text;
        }

        if (loginFin) {
            loginFin = false;
            AttemptingLoginPanel.SetActive(false);
            if (auth.CurrentUser == null) {
                    Debug.Log("Error with sign in try again!");
                toastMaker.showToast("There was an error, please try again", 2);
                    //Error msg
            }
            else{
                    Debug.Log("Sign in succesful!");
                    SceneManager.LoadSceneAsync(1);
            }
        }

        if (regFin) {
            regFin = false;
            AttemptingRegPanel.SetActive(false);
            if (regSuccess) {
                toastMaker.showToast("Account registered successfully. Redirected to Login page", 2);
                OpenLoginMenu();
                regSuccess = false;
            }
            else {
                toastMaker.showToast("Account registration failed", 2);
            }



        }
    }

    //Triggers creation of user on firebase database 
    public void Register() {
        AttemptingRegPanel.SetActive(true);
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                regSuccess = false;
                regFin = true;
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                regSuccess = false;
                regFin = true;
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            createUser(email);
            regSuccess = true;
            regFin = true;
        });
        
    }

    //Method to create the user on datbase 
    public void createUser(String email) {
        Debug.Log("Create user on DB");
        String id = accountRef.Push().Key;
        Debug.Log(reference);
        Debug.Log(reference.Child("users"));
        Debug.Log(id);
        Accounts acc = new Accounts(email, id);

 

        //reference.Child("villages").Child(key).Child("User").SetValueAsync(email);
        //vilRef.Child(id).Child("User").SetValueAsync(email);

        /*
         *  string json = JsonUtility.ToJson(user);

        mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
        */
        String json = JsonUtility.ToJson(acc);
        accountRef.Child(id).SetRawJsonValueAsync(json);

        String dailyBonusID = reference.Child("DailyBonus").Push().Key;
        DailyBonus dailyBon = new DailyBonus(email, dailyBonusID);
        json = JsonUtility.ToJson(dailyBon);
        reference.Child("DailyBonus").Child(dailyBonusID).SetRawJsonValueAsync(json);
    }

    //Checks if login details are authentic  
    public void Login() {
        AttemptingLoginPanel.SetActive(true);

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                loginFin = true;
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                loginFin = true;
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            loginFin = true;
            Debug.Log(loginFin);
        });
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                DebugLog("Signed out " + user.UserId);
                //user is logged out, load login screen 
                //SceneManager.LoadSceneAsync("scene_01");
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                DebugLog("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                DisplayDetailedUserInfo(user, 1);
            }
        }
    }


    // Display a more detailed view of a FirebaseUser.
    void DisplayDetailedUserInfo(Firebase.Auth.FirebaseUser user, int indentLevel)
    {
        DisplayUserInfo(user, indentLevel);
        DebugLog("  Anonymous: " + user.IsAnonymous);
        DebugLog("  Email Verified: " + user.IsEmailVerified);
        var providerDataList = new List<Firebase.Auth.IUserInfo>(user.ProviderData);
        if (providerDataList.Count > 0)
        {
            DebugLog("  Provider Data:");
            foreach (var providerData in user.ProviderData)
            {
                DisplayUserInfo(providerData, indentLevel + 1);
            }
        }
    }

    // Display user information.
    void DisplayUserInfo(Firebase.Auth.IUserInfo userInfo, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        var userProperties = new Dictionary<string, string> {
      {"Display Name", userInfo.DisplayName},
      {"Email", userInfo.Email},
      {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null},
      {"Provider ID", userInfo.ProviderId},
      {"User ID", userInfo.UserId}
    };
        foreach (var property in userProperties)
        {
            if (!String.IsNullOrEmpty(property.Value))
            {
                DebugLog(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
            }
        }
    }

    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s)
    {
        Debug.Log(s);
        logText += s + "\n";

        while (logText.Length > kMaxLogSize)
        {
            int index = logText.IndexOf("\n");
            logText = logText.Substring(index + 1);
        }
    }
}
