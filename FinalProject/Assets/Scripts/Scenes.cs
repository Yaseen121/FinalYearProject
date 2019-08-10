/************************************************************************************************************************************************************************
 * Script used to pass parameters to the scenes 
 * Specifically used to pass the village ids to each village so the corresponding turrets can be used
 * Script written by : "flashmandv" on Unity Forum
 * The script can be found here: https://forum.unity.com/threads/unity-beginner-loadlevel-with-arguments.180925/
 ************************************************************************************************************************************************************************/

using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class Scenes
{

    private static Dictionary<string, string> parameters;

    public static void Load(string sceneName, Dictionary<string, string> parameters = null)
    {
        Scenes.parameters = parameters;
        SceneManager.LoadScene(sceneName);
    }

    public static void Load(string sceneName, string paramKey, string paramValue)
    {
        Scenes.parameters = new Dictionary<string, string>();
        Scenes.parameters.Add(paramKey, paramValue);
        SceneManager.LoadScene(sceneName);
    }

    public static Dictionary<string, string> getSceneParameters()
    {
        return parameters;
    }

    public static string getParam(string paramKey)
    {
        if (parameters == null) return "";
        return parameters[paramKey];
    }

    public static void setParam(string paramKey, string paramValue)
    {
        if (parameters == null)
            Scenes.parameters = new Dictionary<string, string>();
        Scenes.parameters.Add(paramKey, paramValue);
    }
}
