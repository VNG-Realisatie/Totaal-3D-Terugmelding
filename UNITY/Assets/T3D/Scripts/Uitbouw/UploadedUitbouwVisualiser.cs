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
        var cityJSON = GetComponent<CityJSON>();
        foreach (var obj in cityJSON.CityObjects)
        {
            obj.transform.SetParent(uitbouw.transform, false);
            var absoluteCenter = cityJSON.AbsoluteCenter;
            obj.transform.position -= new Vector3((float)absoluteCenter.x, (float)absoluteCenter.z, (float)absoluteCenter.y);
        }

        uitbouw.ReparentToMainBuilding(RestrictionChecker.ActiveBuilding.MainCityObject);

        uitbouw.RecalculateBounds(cityJSON.CityObjects);
        uitbouw.SetMeshOffset();

        uitbouw.InitializeUserMovementAxes();

        HasLoaded = true;
    }
}
