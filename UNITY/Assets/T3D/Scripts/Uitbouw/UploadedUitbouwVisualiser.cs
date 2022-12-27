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
            var offset = GetRelativeCenter(absoluteCenter, cityJSON.CoordinateSystem);
            obj.transform.position -= offset;
        }

        uitbouw.ReparentToMainBuilding(RestrictionChecker.ActiveBuilding.MainCityObject);

        uitbouw.RecalculateBounds(cityJSON.CityObjects);
        uitbouw.SetMeshOffset();

        uitbouw.InitializeUserMovementAxes();

        HasLoaded = true;
    }

    private Vector3 GetRelativeCenter(Vector3Double center, CoordinateSystem coordinateSystem)
    {
        switch (coordinateSystem)
        {
            case CoordinateSystem.WGS84:
                var wgs = new Vector3WGS(center.x, center.y, center.z);
                return CoordConvert.WGS84toUnity(wgs);
            case CoordinateSystem.RD:
                var rd = new Vector3RD(center.x, center.y, center.z);
                return CoordConvert.RDtoUnity(rd);
        }
        return new Vector3((float)center.x, (float)center.z, (float)center.y);
    }
}
