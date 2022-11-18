using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Netherlands3D.Core;
using Netherlands3D.Events;
using Netherlands3D.Interface;
using Netherlands3D.Utilities;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace T3D.Uitbouw
{
    public class ObjectDataEventArgs : EventArgs
    {
        public bool IsLoaded { get; private set; }
        public Vector3 TileOffset;
    }

    public class PerceelDataEventArgs : EventArgs
    {
        public bool IsLoaded { get; private set; }
        public List<Vector2[]> Perceel { get; private set; } //in RD coordinaten
        public float Area { get; private set; }

        public Vector2RD Center;
        public float Radius;

        public PerceelDataEventArgs(bool isLoaded, List<Vector2[]> perceel, float area)
        {
            IsLoaded = isLoaded;
            Perceel = perceel;
            Area = area;

            var centerAndRadius = GeometryCalculator.GetCenterAndRadius(perceel);
            Center = new Vector2RD(centerAndRadius.Center.x, centerAndRadius.Center.y);
            Radius = centerAndRadius.Radius;
        }
    }

    public class BuildingOutlineEventArgs : EventArgs
    {
        public bool IsLoaded { get; private set; }
        public List<Vector2[]> Outline { get; private set; } //in RD coordinaten
        public float TotalArea { get; private set; }

        public Vector2RD Center;
        public float Radius;

        public BuildingOutlineEventArgs(bool isLoaded, List<Vector2[]> outline, float totalArea)
        {
            IsLoaded = isLoaded;
            Outline = outline;
            TotalArea = totalArea;

            var centerAndRadius = GeometryCalculator.GetCenterAndRadius(outline);
            Center = new Vector2RD(centerAndRadius.Center.x, centerAndRadius.Center.y);
            Radius = centerAndRadius.Radius;

        }
    }

    public class MetadataLoader : MonoBehaviour, IUniqueService
    {
        [SerializeField]
        private GameObject shapableUitbouw;
        [SerializeField]
        private GameObject uploadedUitbouwPrefab;

        public delegate void BuildingMetaDataLoadedEventHandler(object source, ObjectDataEventArgs args);
        public event BuildingMetaDataLoadedEventHandler BuildingMetaDataLoaded;

        public delegate void CityJsonBagLoadedEventHandler(object source, string sourceJson, Mesh mesh);

        public delegate void PerceelDataLoadedEventHandler(object source, PerceelDataEventArgs args);
        public event PerceelDataLoadedEventHandler PerceelDataLoaded;

        public delegate void BuildingOutlineLoadedEventHandler(object source, BuildingOutlineEventArgs args);
        public event BuildingOutlineLoadedEventHandler BuildingOutlineLoaded;

        public delegate void CityJsonBagBoundingBoxEventHandler(string cityJson, string excludeBagId);
        public event CityJsonBagBoundingBoxEventHandler CityJsonBagBoundingBoxReceived;

        [SerializeField]
        private BuildingMeshGenerator building;
        [SerializeField]
        private PerceelRenderer perceel;

        //todo: separate?
        public static UitbouwBase Uitbouw { get; private set; }
        public static BuildingMeshGenerator Building { get; private set; }
        public static PerceelRenderer Perceel { get; private set; }

        public string CityJsonBag { get; private set; }

        void Awake()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Building = building;
            Perceel = perceel;
        }

        public void RequestPerceelAndBuildingOutlineData(Vector3RD position, string bagId)
        {
            StartCoroutine(GetPerceelData(position));
            StartCoroutine(RequestBuildingOutlineData(bagId));
        }

        IEnumerator RequestBuildingOutlineData(string bagId)
        {
            var url = $"https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/panden/{bagId}";

            UnityWebRequest req = UnityWebRequest.Get(url);
            req.SetRequestHeader("X-Api-Key", "l772bb9814e5584919b36a91077cdacea7");
            req.SetRequestHeader("Accept-Crs", "epsg:28992");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                ServiceLocator.GetService<WarningDialogs>().ShowNewDialog("Pand data kon niet opgehaald worden");
            }
            else
            {
                var json = JSON.Parse(req.downloadHandler.text);
                //var geometry = json["pand"]["geometrie"]["coordinates"];

                List<Vector2[]> list = new List<Vector2[]>();
                float totalArea = 0f;

                foreach (JSONNode feature in json["pand"]["geometrie"]["coordinates"]) //for each pand op het perceel
                {
                    List<Vector2> polygonList = new List<Vector2>();

                    foreach (JSONNode point in feature)
                    {
                        polygonList.Add(new Vector2(point[0], point[1]));
                    }
                    var polygonArray = polygonList.ToArray();
                    list.Add(polygonArray);
                    totalArea += GeometryCalculator.Area(polygonArray);
                }
                BuildingOutlineLoaded?.Invoke(this, new BuildingOutlineEventArgs(true, list, totalArea));
            }
        }

        IEnumerator GetPerceelData(Vector3RD position)
        {
            print(SessionSaver.HasLoaded);
            yield return new WaitUntil(() => SessionSaver.HasLoaded);// wait until position has loaded

            var bbox = $"{ position.x - 0.5},{ position.y - 0.5},{ position.x + 0.5},{ position.y + 0.5}";
            var url = $"https://geodata.nationaalgeoregister.nl/kadastralekaart/wfs/v4_0?SERVICE=WFS&REQUEST=GetFeature&VERSION=2.0.0&TYPENAMES=kadastralekaartv4:perceel&STARTINDEX=0&COUNT=1&SRSNAME=urn:ogc:def:crs:EPSG::28992&BBOX={bbox},urn:ogc:def:crs:EPSG::28992&outputFormat=json";

            UnityWebRequest req = UnityWebRequest.Get(url);
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                ServiceLocator.GetService<WarningDialogs>().ShowNewDialog("Perceel data kon niet opgehaald worden");
            }
            else
            {
                var json = JSON.Parse(req.downloadHandler.text);
                ProcessPerceelData(json);
            }
        }

        //public IEnumerator GetCityJsonBag(string id)
        //{
        //    var url = $"https://tomcat.totaal3d.nl/happyflow-wfs/wfs?SERVICE=WFS&VERSION=2.0.0&REQUEST=GetFeature&TYPENAMES=bldg:Building&RESOURCEID=NL.IMBAG.Pand.{id}&OUTPUTFORMAT=application%2Fjson";
        //    var uwr = UnityWebRequest.Get(url);

        //    using (uwr)
        //    {
        //        yield return uwr.SendWebRequest();
        //        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        //        {
        //            Debug.LogError(uwr.error);
        //        }
        //        else
        //        {
        //            CityJsonBag = uwr.downloadHandler.text;
        //            cityJsonBagReceived.started.Invoke(CityJsonBag);
        //        }

        //    }
        //}

        //public void LoadTestBuilding(string json)
        //{
        //    Debug.LogError("LOADING TEST BUILDING. Continuing is safe for test purposes only. Do not forget to disable the test building in T3DInit before making build.");
        //    cityJsonBagReceived.started.Invoke(json);
        //}

        void ProcessPerceelData(JSONNode jsonData)
        {
            print(jsonData.ToString());
            JSONNode feature1 = jsonData["features"][0];
            //var perceelGrootte = $"Perceeloppervlakte: {feature1["properties"]["kadastraleGrootteWaarde"]}";

            List<Vector2[]> list = new List<Vector2[]>();

            foreach (JSONNode feature in jsonData["features"])
            {
                List<Vector2> polygonList = new List<Vector2>();

                var coordinates = feature["geometry"]["coordinates"];
                foreach (JSONNode points in coordinates)
                {
                    foreach (JSONNode point in points)
                    {
                        polygonList.Add(new Vector2(point[0], point[1]));
                    }
                }
                list.Add(polygonList.ToArray());
            }

            //PerceelData = list;
            var perceelGrootte = float.Parse(jsonData["features"][0]["properties"]["kadastraleGrootteWaarde"]);
            PerceelDataLoaded?.Invoke(this, new PerceelDataEventArgs(true, list, perceelGrootte));

            //PlaatsUitbouw();
        }

        public void PlaatsUitbouw(Vector3 spawnPosition)
        {
            //var pos = CoordConvert.RDtoUnity(perceelnummerPlaatscoordinaat);
            //if (!Uitbouw)
            //{
            //if (Uitbouw)
            EnableActiveuitbouw(false);

            //if (Uitbouw)
            //{
            //    Uitbouw.GetComponent<UitbouwRotation>().SetAllowRotation(false);
            //    Uitbouw.GetComponent<UitbouwMovement>().SetAllowMovement(false);
            //    Uitbouw.GetComponent<UitbouwMeasurement>().enabled = false;

            //    if (ServiceLocator.GetService<T3DInit>().HTMLData.HasFile)
            //    {
            //        var obj = ServiceLocator.GetService<CityJsonVisualiser>();
            //        obj.EnableUploadedModel(false);
            //    }
            //    else
            //    {

            //        shapableUitbouw.SetActive(false);
            //    }
            //}

            if (ServiceLocator.GetService<T3DInit>().HTMLData.HasFile)
            {
                var visualiser = ServiceLocator.GetService<UploadedUitbouwVisualiser>();
                //obj.VisualizeCityJson();
                //obj.EnableUploadedModel(true);
                Uitbouw = visualiser.GetComponentInChildren<UitbouwBase>(true);
                //Uitbouw.transform.position = spawnPosition;
            }
            else
            {
                //shapableUitbouw.SetActive(true);
                //shapableUitbouw.transform.position = spawnPosition;
                //shapableUitbouw.transform.rotation = Quaternion.identity;
                Uitbouw = shapableUitbouw.GetComponentInChildren<UitbouwBase>(true);
            }

            EnableActiveuitbouw(true);
            Uitbouw.GetComponent<UitbouwMovement>().SetPosition(spawnPosition);
            //}

        }

        //public void EnableUploadedUitbouw(bool active)
        //{
        //    var obj = ServiceLocator.GetService<CityJsonVisualiser>();
        //    obj.EnableUploadedModel(active);
        //}

        //public void EnableShapeableUibouw(bool active)
        //{
        //    shapableUitbouw.SetActive(active);
        //}

        public void EnableActiveuitbouw(bool active)
        {
            if (!Uitbouw)
                return;

            Uitbouw.transform.parent.gameObject.SetActive(active);
            Uitbouw.GetComponent<UitbouwMovement>().SetAllowMovement(active && (State.ActiveState.GetType() == typeof(PlaceUitbouwState))); 
            Uitbouw.GetComponent<UitbouwRotation>().SetAllowRotation(active && (State.ActiveState.GetType() == typeof(PlaceUitbouwState)));
            Uitbouw.EnableGizmo(active && (State.ActiveState.GetType() == typeof(PlaceUitbouwState)));
            //DisableUitbouwToggle.Instance.SetIsOnWithoutNotify(true);
        }
    }
}

