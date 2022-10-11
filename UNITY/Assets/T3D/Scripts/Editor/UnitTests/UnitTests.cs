using Netherlands3D;
using NUnit.Framework;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;
using WebGLFileUploaderExample;


public class UnitTests
{
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

    [UnityTest]
    public IEnumerator TestAsync()
    {
        yield return Run().AsCoroutine();

        async Task Run()
        {
            var result = await AsyncGet();
            Debug.Log(result);
        }
    }

    [UnityTest]
    public IEnumerator TestPost()
    {
        yield return Run().AsCoroutine();

        async Task Run()
        {
            var result = await AsyncPost();
            Debug.Log(result);
        }
    }


}
