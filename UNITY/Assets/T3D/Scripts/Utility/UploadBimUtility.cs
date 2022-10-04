using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class UploadBimUtility 
{

    public static async Task<string> UploadFile(string url, string filePath)
    {        
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        FileInfo finfo = new FileInfo(filePath);
        string data = File.ReadAllText(filePath);

        formData.Add(new MultipartFormFileSection(data, finfo.Name));

        UnityWebRequest req = UnityWebRequest.Post(url, formData);
        req.method = "PUT";

        req.SendWebRequest();

        while (!req.isDone)
        {
            await Task.Yield();
        }

        if (req.result != UnityWebRequest.Result.Success)
        {
            return req.error;            
        }
        else
        {
            return req.downloadHandler.text;            
        }
    }

    public static async Task<string> AsyncGet()
    {        
        string url = "https://api.thecatapi.com/v1/images/search?limit=10";

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SendWebRequest();
        while (!req.isDone)
        {
            await Task.Yield();
        }

        if (req.result != UnityWebRequest.Result.Success)
        {         
            return req.error;
        }
        else
        {            
            return req.downloadHandler.text;
        }
    }

    public static async Task<string> AsyncPost()
    {
        string url = "http://localhost:7071/api/TestPost";

        var json = @"{""name"": ""Trab""}";
        Debug.Log(json);

        UnityWebRequest req = UnityWebRequest.Put(url, json);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SendWebRequest();
        while (!req.isDone)
        {
            await Task.Yield();
        }

        if (req.result != UnityWebRequest.Result.Success)
        {
            return req.error;
        }
        else
        {
            return req.downloadHandler.text;
        }
    }

    public static IEnumerator CheckBimVersion(string url, Action successCallback, Action<string> errorCallback)
    {
        int retryCount = 0;

        while (retryCount < 10)
        {
            UnityWebRequest req = UnityWebRequest.Get(url);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                yield return new WaitForSeconds(1);
            }
            else
            {
                var json = JSON.Parse(req.downloadHandler.text);
                var status = json["conversions"]["cityjson"];

                if (status == "DONE")
                {
                    successCallback.Invoke();
                    yield break;
                }

                yield return new WaitForSeconds(1);
                retryCount++;
            }

        }

        errorCallback?.Invoke($"max retry exceeded on {url} ");

    }


}
