using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionUI : MonoBehaviour
{
    public void StartNewSession()
    {
        SessionSaver.LoadPreviousSession = false;
        if (SceneManager.GetActiveScene() != ErrorService.ErrorScene)
            SessionSaver.ClearAllSaveData();
        RestartT3DScene();
    }

    public void LoadSavedSession()
    {
        SessionSaver.LoadPreviousSession = true;
        RestartT3DScene(); 
    }

    private void RestartT3DScene()
    {
        CityJSONFormatter.Reset();
        //Scene scene = SceneManager.GetSceneByName("T3D");
        SceneManager.LoadScene("T3D");
    }
}
