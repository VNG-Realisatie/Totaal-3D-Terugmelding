#define DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
//using Netherlands3D.Sharing;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SessionSaver
{
    public static bool LoadPreviousSession { get; set; } = true;

    public static JSONSessionLoader Loader { get { return ServiceLocator.GetService<JSONSessionLoader>(); } }
    public static JsonSessionSaver Saver { get { return ServiceLocator.GetService<JsonSessionSaver>(); } }

    public static string SessionId { get; private set; } = string.Empty;
    public static bool HasLoaded => Loader.HasLoaded;

    public static bool SessionExists { get; private set; }

    static SessionSaver()
    {
        if (SessionId == string.Empty)
        {
            if (TryGetSessionID(out string id))
            {
                SessionId = id;
                URLModifier.SetSessionIdInURL(SessionId);
                PlayerPrefs.SetString("sessionId", SessionId);
                SessionExists = true;
            }
            else
            {
                GenerateSessionID();
                SessionExists = false;
            }
        }
        SessionId += "_html";
        Debug.Log("session id: " + SessionId);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!LoadPreviousSession)
            GenerateSessionID();

        if (scene != ErrorService.ErrorScene)
            LoadSaveData(); //This data also includes essential information like bagId, so always load the data
    }

    public static void ClearAllSaveData()
    {
        Saver.ClearAllData(SessionId);
    }

    public static void ExportSavedData()
    {
        Saver.ExportSaveData(SessionId);
    }

    public static void UploadFileToEndpoint()
    {
        Saver.UploadCityJSONFileToEndpoint();
    }

    public static void LoadSaveData()
    {
        Debug.Log("Loading save data for session: " + SessionId);
        Loader.ReadSaveData(SessionId);
        Loader.LoadingCompleted += Loader_LoadingCompleted;
    }

    private static void Loader_LoadingCompleted(bool loadSucceeded)
    {
        if (loadSucceeded)
        {
            Debug.Log("loaded session: " + SessionId);
            ServiceLocator.GetService<T3DInit>().HTMLData.SessionId = SessionId; //needed for when loading from a location that is not the existing session such as the URL or playerprefs
        }
        else
        {
            Debug.Log("loading session data for session " + SessionId + " failed, writing session from Unity");
            ServiceLocator.GetService<T3DInit>().HTMLData.SessionId = SessionId;
            Saver.ExportSaveData(SessionId);
            ServiceLocator.GetService<JsonSessionSaver>().EnableAutoSave(true); //enable autosave
            SessionExists = false;
        }
        //ServiceLocator.GetService<T3DInit>().LoadBuilding();
        Loader.LoadingCompleted -= Loader_LoadingCompleted;
    }

    public static void AddContainer(SaveDataContainer saveDataContainer)
    {
        Saver.AddContainer(saveDataContainer);
    }

    public static void RemoveContainer(SaveDataContainer saveDataContainer)
    {
        Saver.RemoveContainer(saveDataContainer);
    }

    public static void LoadContainer(SaveDataContainer saveDataContainer)
    {
        //check if object already exists in the save data, in which case load the save data:
        if (ServiceLocator.GetService<JSONSessionLoader>().TryGetJson(saveDataContainer.TypeKey, saveDataContainer.InstanceId, out string json))
        {
            JsonUtility.FromJsonOverwrite(json, saveDataContainer);
        }
    }

    public static JSONNode GetJSONNodeOfType(string typeKey)
    {
        return Loader.GetJSONNodeOfType(typeKey);
    }

    private static bool TryGetSessionID(out string id)
    {
        id = string.Empty;
        //first try to get session id from url

#if UNITY_WEBGL && !UNITY_EDITOR
        id = Application.absoluteURL.GetUrlParamValue("sessionId");
        if(!string.IsNullOrEmpty(id))
        {
            Debug.Log("got id from url: {" + id + "}");
            return true;
        }
#endif
        // if this fails: try to get session id from PlayerPrefs
        id = PlayerPrefs.GetString("sessionId");
        if (id != string.Empty)
        {
            Debug.Log("got id from playerPrefs: " + id);
            return true;
        }

        //else: no id found
        Debug.Log("no Session Id found");
        return false;
    }

    private static void GenerateSessionID()
    {
        SessionId = Guid.NewGuid().ToString();
        Debug.Log("Session id not found, generating new session: " + SessionId);
        URLModifier.SetSessionIdInURL(SessionId);
        PlayerPrefs.SetString("sessionId", SessionId);
    }
}
