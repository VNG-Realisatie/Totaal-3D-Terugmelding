﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Netherlands3D;
using Netherlands3D.Interface;
using SimpleJSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class JsonSessionSaver : MonoBehaviour, IUniqueService//, IDataSaver
{
    private JSONNode rootObject = new JSONObject();
    const string uploadURL = "api/upload/";

    public Coroutine saveCoroutine;    
    private bool autoSaveEnabled = true;
    [SerializeField]
    private SaveFeedback saveFeedback;

    public event IDataSaver.DataSavedEventHandler SavingCompleted;
    public event IDataSaver.DataSavedEventHandler UploadToEndpointCompleted;
    public bool SaveInProgress => saveCoroutine != null;

    private List<SaveDataContainer> saveDataContainers = new List<SaveDataContainer>();

    public void EnableAutoSave(bool enable)
    {
        autoSaveEnabled = enable && ServiceLocator.GetService<T3DInit>().IsEditMode;
        if (enable)
            StartCoroutine(AutoSaveTimer());
    }

    public void ExportSaveData(string sessionId)
    {
        string saveData = SerializeSaveableContainers();
      //  print(saveData);

        PlayerPrefs.SetString(sessionId, saveData);

        if (saveCoroutine == null)
        {
            saveCoroutine = StartCoroutine(UploadData(sessionId, saveData));
        }
        else
        {
            print("Still waiting for coroutine to return, not saving data");
            SavingCompleted?.Invoke(false);
        }
    }

    public void UploadCityJSONFileToEndpoint()
    {
        string saveData = CityJSONFormatter.GetJSON();
        print(saveData);
        StartCoroutine(GetVersionAndUpload(saveData));        
    }

    IEnumerator GetVersionAndUpload(string saveData)
    {
        CoString versie = new CoString();
        yield return GetVersie(versie);        
        yield return UploadDataToEndpoint(saveData, versie);
    }

    IEnumerator GetVersie(CoString versie)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        var url = Path.Combine(Application.dataPath, "versie.txt");
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
        {
            ServiceLocator.GetService<WarningDialogs>().ShowNewDialog("versie.txt kon niet opgehaald worden");
        }
        else
        {
            versie.val = req.downloadHandler.text;            
        }
#else        
        versie.val = PlayerSettings.bundleVersion;
        yield return null;
#endif
    }

    public void AddContainer(SaveDataContainer saveDataContainer)
    {
        saveDataContainers.Add(saveDataContainer);
    }

    public void RemoveContainer(SaveDataContainer saveDataContainer)
    {
        saveDataContainers.Remove(saveDataContainer);
    }

    public string SerializeSaveableContainers()
    {
        rootObject = new JSONObject(); //delete any old data that may be in the rootObject
        foreach (var container in saveDataContainers)
        {
            string jsonContent = JsonUtility.ToJson(container); // Base container's derivative class content variables
            var node = JSONNode.Parse(jsonContent); //todo : not seralize and deserialize here
            rootObject[container.TypeKey].Add(container.InstanceId, node);
        }

        return rootObject.ToString();
    }

    private IEnumerator UploadData(string name, string data)
    {
        var url = Config.activeConfiguration.T3DAzureFunctionURL + uploadURL + name;
        var uwr = UnityWebRequest.Put(url, data);

        using (uwr)
        {
            saveFeedback.SetSaveStatus(SaveFeedback.SaveStatus.Saving);
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(uwr.error);
                SavingCompleted?.Invoke(false);
            }
            else
            {
                saveFeedback.SetSaveStatus(SaveFeedback.SaveStatus.ChangesSaved);
                SavingCompleted?.Invoke(true);
            }
            saveCoroutine = null;
        }
    }

    private IEnumerator UploadDataToEndpoint(string jsonData, string versie)
    {
        //var url = Config.activeConfiguration.T3DAzureFunctionURL + uploadURL + name;
        var url = Config.activeConfiguration.CityJSONUploadEndoint;
        var uwr = UnityWebRequest.Put(url, jsonData);
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("objectId", ServiceLocator.GetService<T3DInit>().HTMLData.BagId);
        uwr.SetRequestHeader("initiatorPersoon", SubmitPermitRequestState.UserName);
        uwr.SetRequestHeader("Authorization", "Bearer " + Config.activeConfiguration.CityJSONUploadEndpointToken);
        uwr.SetRequestHeader("initiatieSysteemVersie", versie );

        yield return null;

        using (uwr)
        {
            //saveFeedback.SetSaveStatus(SaveFeedback.SaveStatus.Saving);
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(uwr.error);
                UploadToEndpointCompleted?.Invoke(false);
            }
            else
            {
                //saveFeedback.SetSaveStatus(SaveFeedback.SaveStatus.ChangesSaved);
                UploadToEndpointCompleted?.Invoke(true);
            }            
        }
    }

    public void ClearAllData(string sessionId)
    {
        var persistentTypes = KeepSaveDataOnResetAttribute.GetPersistentTypeKeys();
        List<SaveDataContainer> persistentContainers = new List<SaveDataContainer>();

        foreach (var container in saveDataContainers)
        {
            if (persistentTypes.Contains(container.TypeKey))
            {
                persistentContainers.Add(container);
            }
        }

        saveDataContainers = persistentContainers;
        saveFeedback.SetSaveStatus(SaveFeedback.SaveStatus.WaitingToSave);
        ExportSaveData(sessionId);
    }

    private IEnumerator AutoSaveTimer()
    {
        while (autoSaveEnabled)
        {
            yield return new WaitForSeconds(5f);
            ExportSaveData(SessionSaver.SessionId);
        }
    }
}
