using Netherlands3D;
using Netherlands3D.T3D.Uitbouw;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UploadBimUtility 
{

    public static IEnumerator UploadFile(CoString result, CoBool status, Slider slider, string url, string filePath)
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
            slider.value = req.uploadProgress;            
            yield return null;
        }

        status.val = req.result == UnityWebRequest.Result.Success;
        result.val = status.val ? req.downloadHandler.text : req.error;

    }

    public static IEnumerator CheckBimVersion(string url, CoString result, CoBool success)
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
                var conversionStatus = json["conversions"]["cityjson"];

                if (conversionStatus == "DONE")
                {
                    result.val = "DONE";
                    success.val = true;
                    yield break;
                }

                yield return new WaitForSeconds(1);
                retryCount++;
            }

        }
        result.val = $"max retry exceeded on {url} ";
    }
    
    public static IEnumerator GetBimCityJson(CoString result, CoBool success)
    {
        yield return null;

        var urlIfc = Config.activeConfiguration.T3DAzureFunctionURL + $"api/getbimcityjson/{ServiceLocator.GetService<T3DInit>().HTMLData.ModelId}";
        var urlSketchup = Config.activeConfiguration.T3DAzureFunctionURL + $"api/downloadcityjson/{ServiceLocator.GetService<T3DInit>().HTMLData.BlobId}";

        var requestUrl = !string.IsNullOrEmpty(ServiceLocator.GetService<T3DInit>().HTMLData.ModelId) ? urlIfc : urlSketchup;

        UnityWebRequest req = UnityWebRequest.Get(requestUrl);

        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
        {
            result.val = req.error;            
        }
        else
        {
            result.val = req.downloadHandler.text;
            success.val = true;                        
        }
    }


}
