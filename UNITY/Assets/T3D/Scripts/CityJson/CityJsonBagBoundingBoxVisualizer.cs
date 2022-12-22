using Netherlands3D.Core;
using Netherlands3D.Events;
using T3D.Uitbouw;
using Netherlands3D.T3DPipeline;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class CityJsonBagBoundingBoxVisualizer : MonoBehaviour
{
    [SerializeField]
    private DoubleArrayEvent onBuildingPositionReceived;
    [SerializeField]
    private StringEvent boundingBoxCityJSONReceived;
    [SerializeField]
    private TriggerEvent onBoundingBoxCityJSONVisualized;
    private string excludeBagID;

    void OnEnable()
    {
        onBuildingPositionReceived.started.AddListener(RequestBoundingBoxJSON);
    }

    void OnDisable()
    {
        onBuildingPositionReceived.started.RemoveAllListeners();
    }

    public void RequestBoundingBoxJSON(double[] pos)
    {
        //var rdPos = new Vector3RD(pos[0], pos[1], pos[2]);
        var bagId = ServiceLocator.GetService<T3DInit>().HTMLData.BagId;
        StartCoroutine(GetCityJsonBagBoundingBox(pos[0], pos[1], bagId));
    }

    private IEnumerator GetCityJsonBagBoundingBox(double x, double y, string excludeBagId)
    {
        string bbox = $"{x - 25},{y - 25},{x + 25},{y + 25}";

        var url = $"https://tomcat.totaal3d.nl/happyflow-wfs/wfs?SERVICE=WFS&VERSION=2.0.0&REQUEST=GetFeature&TYPENAMES=bldg:Building&BBOX={bbox}&OUTPUTFORMAT=application%2Fjson";
        var uwr = UnityWebRequest.Get(url);

        using (uwr)
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("WebRequest failed: Could not load buildings in bounding box");
            }
            else
            {
                excludeBagID = excludeBagId;
                onBoundingBoxCityJSONVisualized.started.AddListener(RemoveBuildingsFromExport); //add listener before invoking the event that triggers the parsing (and visualization by extension)
                onBoundingBoxCityJSONVisualized.started.AddListener(DisableMainBuilding); //add listener before invoking the event that triggers the parsing (and visualization by extension)
                boundingBoxCityJSONReceived.Invoke(uwr.downloadHandler.text);
            }
        }
    }

    private void RemoveBuildingsFromExport()
    {
        var cityObjects = GetComponent<CityJSON>().CityObjects;
        foreach(var co in cityObjects)
        {
            co.IncludeInExport = false;
        }
    }

    public void DisableMainBuilding()
    {
        var mainBuildingsInBoundingBox = GetComponent<CityJSON>().CityObjects.Where(co => co.Id.Contains(excludeBagID));
        foreach (var mainBuilding in mainBuildingsInBoundingBox)
        {
            mainBuilding.gameObject.SetActive(false);
        }
        onBoundingBoxCityJSONVisualized.started.RemoveAllListeners();
    }
}
