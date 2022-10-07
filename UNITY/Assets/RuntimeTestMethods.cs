using Netherlands3D;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomEditor(typeof(RuntimeTestMethods)), CanEditMultipleObjects()]
public class RuntimeTestMethodsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RuntimeTestMethods rtm = (RuntimeTestMethods)target;

        if (GUILayout.Button("Converteer lokaal IFC naar CityJson"))
        {
            rtm.TestConvert();
        }
    }
}
#endif

public class RuntimeTestMethods : MonoBehaviour
{

    internal void TestConvert()
    {
        StartCoroutine(StartConvert());
    }

    IEnumerator StartConvert()
    {
        //string filepath = @"E:\T3D\Data\IFC\ASP9 - Nieuw.ifc";
        string filepath = @"E:\T3D\Data\IFC modellen voor_op_site\Uitbouw -  in lengte.ifc";

        FileInfo finfo = new FileInfo(filepath);
        var url = ($"https://t3d-o-functions.azurewebsites.net/api/uploadbim/{Uri.EscapeDataString(finfo.Name)}");

        yield return StartCoroutine( TestUploadIfcFile(url, filepath));
        
    }


    IEnumerator TestUploadIfcFile(string url, string filePath)
    {
        CoString result = new CoString();
        CoBool success = new CoBool();

        yield return StartCoroutine(UploadBimUtility.UploadFile(result, success, url, filePath));
        Debug.Log(result);

        if (success == false) yield break;

        var jsonResult = JSON.Parse(result);

        FileInfo finfo = new FileInfo(filePath);
        if (finfo.Extension.ToLower() == "skp")
        {
            ServiceLocator.GetService<T3DInit>().HTMLData.BlobId = jsonResult["blobId"];
        }
        else
        {
            ServiceLocator.GetService<T3DInit>().HTMLData.ModelId = jsonResult["modelId"];
            var urlIfc = Config.activeConfiguration.T3DAzureFunctionURL + $"api/getbimversionstatus/{ServiceLocator.GetService<T3DInit>().HTMLData.ModelId}";

            yield return StartCoroutine(UploadBimUtility.CheckBimVersion(urlIfc, result, success));

            Debug.Log(result);

            //File is successfuly converted and ready to download, let's get it
            if (success)
            {
                yield return StartCoroutine(UploadBimUtility.GetBimCityJson(result, success));

                //The CityJson has been downloaded, now lets visualize it
                if (success)
                {
                    Debug.Log("-------BimCityJsonReceived");
                    ServiceLocator.GetService<Events>().RaiseBimCityJsonReceived(result);
                }
                else
                {
                    Debug.LogError(result);                    
                }
            }

        }

    }
}
