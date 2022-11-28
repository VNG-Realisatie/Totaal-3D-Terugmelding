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
        ServiceLocator.GetService<Events>().BimCityJsonReceived += OnBimCityJsonReceived;
        onUploadedModelVisualized.started.AddListener(OnUploadedModelVisualized);
    }

    void OnDisable()
    {
        ServiceLocator.GetService<Events>().BimCityJsonReceived -= OnBimCityJsonReceived;
        onUploadedModelVisualized.started.RemoveListener(OnUploadedModelVisualized);
    }

    private void OnBimCityJsonReceived(string cityJson)
    {
        onBimCityJsonReceived.Invoke(cityJson);
    }

    private void OnUploadedModelVisualized()
    {
        Mesh combinedActiveMesh = new Mesh();
        foreach (var obj in GetComponent<CityJSON>().CityObjects)
        {
            obj.transform.SetParent(GetComponentInChildren<UploadedUitbouw>(true).transform);
            var visualizer = obj.GetComponent<CityObjectVisualizer>();
            var mesh = visualizer.ActiveMesh;
            var meshList = new List<Mesh>();
            meshList.Add(combinedActiveMesh);
            meshList.Add(mesh);
            combinedActiveMesh = CityObjectVisualizer.CombineMeshes(meshList, obj.transform.localToWorldMatrix);
        }

        uitbouw.ReparentToMainBuilding(RestrictionChecker.ActiveBuilding.MainCityObject);

        uitbouw.RecalculateBounds();
        uitbouw.SetMeshOffset();

        uitbouw.InitializeUserMovementAxes();

        HasLoaded = true;
    }
}
