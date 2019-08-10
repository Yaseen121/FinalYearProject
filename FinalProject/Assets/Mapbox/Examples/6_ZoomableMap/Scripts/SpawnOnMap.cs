/************************************************************************************************************************************************************************
 * Script attached to the map object on the world scene
 * The original script is supplied with the mapbox library but has been heavily modified 
 * This script was used to spawn villages on the map 
 * This documentation can be found here: https://docs.mapbox.com/unity/maps/examples/zoomable-map/
 * All other code has been written/modified by: Mohammed Yaseen Sultan
 ************************************************************************************************************************************************************************/


namespace Mapbox.Examples
{
    using UnityEngine;
    using Mapbox.Utils;
    using Mapbox.Unity.Map;
    using Mapbox.Unity.MeshGeneration.Factories;
    using Mapbox.Unity.Utilities;
    using System.Collections.Generic;
    using Firebase.Database;
    using Firebase;
    using Firebase.Unity.Editor;
    using UnityEngine.UI;

    public class SpawnOnMap : MonoBehaviour
    {
        [SerializeField]
        AbstractMap _map;

        [SerializeField]
        [Geocode]
        private string[] _locationStrings;
        Vector2d[] _locations;

        [SerializeField]
        float _spawnScale = 100f;

        [SerializeField]
        GameObject _markerPrefab;

        List<GameObject> _spawnedObjects;

        private DatabaseReference reference;
        private List<Villages> locationStringsAL;

        public GameObject loadingPanel;

        private bool loadDone = false;
        private bool init = false;

        public Text userEmail;

        [SerializeField]
        GameObject myVill;
        [SerializeField]
        GameObject oppVil;

        //Returns the correct village model 
        //This is depended on the logged in user
        //So it returns the myVill (blue village) if the logged in user is the same as the owner of the village being checked
        //else it returns oppVil (red village) as it means its an enemy village 
        public  GameObject getPrefab(string ownerEmail)
        {
            if (ownerEmail.Equals(userEmail.text))
            {
                return myVill;
            }
            else
            {
                return oppVil;
            }
        }

        //Activates/displays loading panel while villages are being loaded into a list 
        void Start()
        {
            loadingPanel.SetActive(true);
            // Set up the Editor before calling into the realtime database.
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://finalprojectunity-31171.firebaseio.com//");
            // Get the root reference location of the database.
            reference = FirebaseDatabase.DefaultInstance.RootReference;

            locationStringsAL = new List<Villages>();
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
                        //Debug.Log("Here: " + s.Child("email").Value + " " + s.Child("lat").Value + " " + s.Child("lng").Value);
                        //Villages vil = new Villages(s.Child("email").Value.ToString(), s.Child("lat").Value.ToString(), s.Child("lng").Value.ToString(), s.Child("id").Value.ToString());
                        Villages vil = new Villages(s.Child("email").Value.ToString(), s.Child("lat").Value.ToString(), s.Child("lng").Value.ToString(), s.Child("id").Value.ToString(), s.Child("size").Value.ToString());
                        locationStringsAL.Add(vil);
                    }
                    Debug.Log("Done Inside Firebase !!");
                    loadDone = true;
                }
            });
        }

        //Once villages have finished being loaded 
        //Villages are rendered/instantiated on the map and the radius is adjusted to fit the size of the village 
        //The ids of villages are set as they are being rendered 
        private void Update()
        {
            if (loadDone && !init)
            {
                _locations = new Vector2d[_locationStrings.Length];
                _spawnedObjects = new List<GameObject>();
                for (int i = 0; i < locationStringsAL.Count; i++)
                {
                    var location = Conversions.StringToLatLon(locationStringsAL[i].getLat() + ", " + locationStringsAL[i].getLng());
                    //var instance = Instantiate(getPrefab(locationStringsAL[i].getEmail()));
                    GameObject instance = Instantiate(getPrefab(locationStringsAL[i].getEmail()));
                    Transform radScale = instance.transform.Find("RadiusScale");
                    radScale.localScale += new Vector3((locationStringsAL[i].getSize()-1), 0, (locationStringsAL[i].getSize() - 1));
                    //var instance = Instantiate(_markerPrefab);
                    Village prefabScript = instance.GetComponent<Village>();
                    prefabScript.setID(locationStringsAL[i].getEmail(), locationStringsAL[i].getVillageID());
                    instance.transform.localPosition = _map.GeoToWorldPosition(location, true);
                    instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                    _spawnedObjects.Add(instance);
                }
                init = true;
                loadingPanel.SetActive(false);
            }

            if (loadDone && init)
            {
                for (int i = 0; i < locationStringsAL.Count; i++)
                {
                    var spawnedObject = _spawnedObjects[i];
                    var location = Conversions.StringToLatLon(locationStringsAL[i].getLat() + ", " + locationStringsAL[i].getLng());
                    //Debug.Log("Right Here: " + i);
                    spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                    spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                }
            }

        }
    }
}