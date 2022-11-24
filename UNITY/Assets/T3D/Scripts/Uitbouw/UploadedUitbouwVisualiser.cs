using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Netherlands3D.Core;
using T3D.Uitbouw;
using T3D.LoadData;
using UnityEngine;
using Netherlands3D.T3DPipeline;
using Netherlands3D.Events;

public class UploadedUitbouwVisualiser : MonoBehaviour, IUniqueService
{
    public Material MeshMaterial;
    //private Vector3RD? perceelCenter;
    private string cityJson = string.Empty;
    private UploadedUitbouw uitbouw;

    public bool HasLoaded { get; private set; }

    [SerializeField]
    private TextAsset testJSON;
    //[SerializeField]
    //private bool useTestJSON;
    [SerializeField]
    private StringEvent onBimCityJsonReceived;
    [SerializeField]
    private TriggerEvent onUploadedModelVisualized;

    private void Awake()
    {
        uitbouw = GetComponentInChildren<UploadedUitbouw>(true);
    }

    void OnEnable()
    {
        ServiceLocator.GetService<Events>().BimCityJsonReceived += OnBimCityJsonReceived;
        onUploadedModelVisualized.started.AddListener(OnUploadedModelVisualized);
        //ServiceLocator.GetService<MetadataLoader>().PerceelDataLoaded += OnPerceelDataLoaded;
    }

    void OnDisable()
    {
        ServiceLocator.GetService<Events>().BimCityJsonReceived -= OnBimCityJsonReceived;
        onUploadedModelVisualized.started.RemoveListener(OnUploadedModelVisualized);
        //ServiceLocator.GetService<MetadataLoader>().PerceelDataLoaded -= OnPerceelDataLoaded;
    }

    private void OnBimCityJsonReceived(string cityJson)
    {
        Debug.Log("-OnBimCityJsonReceived");
        //this.cityJson = cityJson;
        //uitbouw.gameObject.SetActive(true);
        onBimCityJsonReceived.Invoke(cityJson);
        //uitbouw.GetComponentInChildren<CityJSON>().ParseCityJSON(cityJson);

        //ParseCityJson(false);
        //VisualizeCityJson();
    }

    private void OnUploadedModelVisualized()
    {
        var uitbouwMeshes = new List<Mesh>();
        foreach (var obj in GetComponent<CityJSON>().CityObjects)
        {
            var visualizer = obj.GetComponent<CityObjectVisualizer>();
            var mesh = visualizer.ActiveMesh;
            uitbouwMeshes.Add(mesh);
        }

        uitbouw.ReparentToMainBuilding(RestrictionChecker.ActiveBuilding.MainCityObject);

        var combinedActiveMesh = CityJsonVisualiser.CombineMeshes(uitbouwMeshes, transform.localToWorldMatrix);
        uitbouw.SetCombinedMesh(combinedActiveMesh);

        //center mesh
        var offset = uitbouw.MeshFilter.mesh.bounds.center;
        offset.y -= uitbouw.MeshFilter.mesh.bounds.extents.y;
        offset += uitbouw.transform.forward * uitbouw.Depth / 2;
        uitbouw.MeshFilter.transform.localPosition = -offset;

        uitbouw.InitializeUserMovementAxes();

        HasLoaded = true;
    }

    public void ParseCityJson(bool useTestJson)
    {
        if (useTestJson)
            cityJson = testJSON.text;

        var meshFilter = uitbouw.MeshFilter;
        var cityJsonModel = new CityJsonModel(cityJson, new Vector3RD(), true);
        var meshes = CityJsonVisualiser.ParseCityJson(cityJsonModel, meshFilter.transform.localToWorldMatrix, false, false);
        var attributes = CityJsonVisualiser.GetAttributes(cityJsonModel.cityjsonNode["CityObjects"]);
        CityJsonVisualiser.AddExtensionNodes(cityJsonModel.cityjsonNode);
        //var combinedMesh = CombineMeshes(meshes.Values.ToList(), meshFilter.transform.localToWorldMatrix);

        //var cityObject = meshFilter.gameObject.AddComponent<CityJSONToCityObject>();
        var highestLod = meshes.Keys.Max(k => k.Lod);
        print("Enabling the highest lod: " + highestLod);

        var parent = meshFilter.transform;
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
        //todo:var cityObjects = CityJSONToCityObject.CreateCityObjects(parent, meshes, attributes, cityJsonModel.vertices);
        var cityObjects = new Netherlands3D.T3DPipeline.CityObject[0];
        var uitbouwMeshes = new List<Mesh>();
        foreach (var obj in cityObjects)
        {
            //todo: uitbouw.AddCityObject(obj.Value);
            //todo: var mesh = obj.Value.SetMeshActive(highestLod);
            //todo: uitbouwMeshes.Add(mesh);
        }
        //var mainBuildingCityObjects = RestrictionChecker.ActiveBuilding.GetComponentsInChildren<CityObject>();
        //var mainBuilding = mainBuildingCityObjects.FirstOrDefault(co => co.Type == CityObjectType.Building);
        uitbouw.ReparentToMainBuilding(RestrictionChecker.ActiveBuilding.MainCityObject);

        var combinedActiveMesh = CityJsonVisualiser.CombineMeshes(uitbouwMeshes, transform.localToWorldMatrix);
        uitbouw.SetCombinedMesh(combinedActiveMesh); 

        //center mesh
        var offset = uitbouw.MeshFilter.mesh.bounds.center;
        offset.y -= uitbouw.MeshFilter.mesh.bounds.extents.y;
        offset += uitbouw.transform.forward * uitbouw.Depth / 2;
        uitbouw.MeshFilter.transform.localPosition = -offset;

        //re-initialize the usermovementaxes to ensure the new meshes are dragable
        //EnableUploadedModel(true);

        //var depthOffset = -transform.forward * uitbouw.Depth / 2;
        //var heightOffset = transform.up * ((uitbouw.Height / 2) - Vector3.Distance(uitbouw.CenterPoint, transform.position));
        //uitbouw.MeshFilter.transform.localPosition = depthOffset + heightOffset;

        uitbouw.InitializeUserMovementAxes();

        //uitbouw.gameObject.transform.parent.gameObject will be active in next state

        HasLoaded = true;
    }
}
