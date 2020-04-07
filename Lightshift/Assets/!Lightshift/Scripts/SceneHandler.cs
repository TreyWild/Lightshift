using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneHandler
{
    #region Room Loading

    /// <summary>
    /// Loading Scenes has issues sometimes where it fires before the scene is fully loaded. Here is a work around.
    /// Call LoadScene to change your scene, and use the delegate for firing your loaded events.
    /// </summary>


    private static Action _sceneLoaded;

    public static void LoadScene(string scene, Action callback)
    {
        _sceneLoaded = callback;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.LoadSceneAsync(scene);
    }

    private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _sceneLoaded?.Invoke();
    }
    #endregion
}
