using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConvertCoordinates;
using Netherlands3D.T3D.Uitbouw;
using T3D.LoadData;
using T3D.Uitbouw;
using UnityEngine;

public class UploadedUitbouwVisualiser : MonoBehaviour, IUniqueService
{
    public Material MeshMaterial;
    //private Vector3RD? perceelCenter;
    private string cityJson = string.Empty;
    private UploadedUitbouw uitbouw;

    public bool HasLoaded { get; private set; }

    [SerializeField]
    private TextAsset testJSON;
    [SerializeField]
    private bool useTestJSON;

    private void Awake()
    {
        uitbouw = GetComponentInChildren<UploadedUitbouw>(true);
    }

    void OnEnable()
    {
        ServiceLocator.GetService<MetadataLoader>().BimCityJsonReceived += OnBimCityJsonReceived;
        //ServiceLocator.GetService<MetadataLoader>().PerceelDataLoaded += OnPerceelDataLoaded;
    }

    void OnDisable()
    {
        ServiceLocator.GetService<MetadataLoader>().BimCityJsonReceived -= OnBimCityJsonReceived;
        //ServiceLocator.GetService<MetadataLoader>().PerceelDataLoaded -= OnPerceelDataLoaded;
    }

    //private void OnPerceelDataLoaded(object source, PerceelDataEventArgs args)
    //{
    //    perceelCenter = new Vector3RD(args.Center.x, args.Center.y, 0);
    //}

    private void OnBimCityJsonReceived(string cityJson)
    {
        Debug.Log("OnBimCityJsonReceived");
        this.cityJson = cityJson;

        VisualizeCityJson();
    }

    public void VisualizeCityJson()
    {
        ParseCityJson(useTestJSON);
    }

    private void ParseCityJson(bool useTestJson)
    {
        if (useTestJSON)
            cityJson = testJSON.text;

        var meshFilter = uitbouw.MeshFilter;
        var cityJsonModel = new CityJsonModel(cityJson, new Vector3RD(), true);
        var meshes = CityJsonVisualiser.ParseCityJson(cityJsonModel, meshFilter.transform.localToWorldMatrix, false, false);
        var attributes = CityJsonVisualiser.GetAttributes(cityJsonModel.cityjsonNode["CityObjects"]);
        CityJsonVisualiser.AddExtensionNodes(cityJsonModel.cityjsonNode);
        //var combinedMesh = CombineMeshes(meshes.Values.ToList(), meshFilter.transform.localToWorldMatrix);

        var cityObject = meshFilter.gameObject.AddComponent<CityJSONToCityObject>();
        cityObject.SetNodes(meshes, attributes, cityJsonModel.vertices);
        uitbouw.AddCityObject(cityObject);
        uitbouw.ReparentToMainBuilding(RestrictionChecker.ActiveBuilding.GetComponent<CityObject>());
        var highestLod = meshes.Keys.Max(k => k.Lod);
        print("Enabling the highest lod: " + highestLod);
        cityObject.SetMeshActive(highestLod);

        //meshFilter.mesh = combinedMesh;
        //uitbouw.GetComponentInChildren<MeshCollider>().sharedMesh = meshFilter.mesh;

        uitbouw.SetMeshFilter(uitbouw.MeshFilter);

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

        HasLoaded = true;
    }
}
