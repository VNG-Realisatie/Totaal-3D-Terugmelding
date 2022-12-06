using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Netherlands3D.Core;
using T3D.Uitbouw;
using UnityEngine;
using Netherlands3D.T3DPipeline;
using Netherlands3D.Events;

public class UploadedUitbouwVisualiser : MonoBehaviour, IUniqueService
{
    public Material MeshMaterial;
    private UploadedUitbouw uitbouw;

    public bool HasLoaded { get; private set; }

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
        onUploadedModelVisualized.started.AddListener(OnUploadedModelVisualized);
    }

    void OnDisable()
    {
        onUploadedModelVisualized.started.RemoveListener(OnUploadedModelVisualized);
    }

    private void OnUploadedModelVisualized()
    {
        //Mesh combinedActiveMesh = new Mesh();
        //var uitbouw = GetComponentInChildren<UploadedUitbouw>(true)
        //transform.position = Vector3.zero;
        //print(transform.position);
        //uitbouw.transform.localPosition = Vector3.zero;
        var cityObjects = GetComponent<CityJSON>().CityObjects;
        foreach (var obj in cityObjects)
        {
            obj.transform.SetParent(uitbouw.transform, false);
            //var visualizer = obj.GetComponent<CityObjectVisualizer>();
            //var mesh = visualizer.ActiveMesh;
            //var meshList = new List<Mesh>();
            //meshList.Add(combinedActiveMesh);
            //meshList.Add(mesh);
            //combinedActiveMesh = CityObjectVisualizer.CombineMeshes(meshList, obj.transform.localToWorldMatrix);
        }

        uitbouw.ReparentToMainBuilding(RestrictionChecker.ActiveBuilding.MainCityObject);

        uitbouw.RecalculateBounds(cityObjects);
        uitbouw.SetMeshOffset();

        uitbouw.InitializeUserMovementAxes();

        HasLoaded = true;
    }
}
