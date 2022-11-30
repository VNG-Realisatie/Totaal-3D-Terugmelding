using System.Collections;
using System.Collections.Generic;
using T3D.Uitbouw;
using UnityEngine;

public class BuildingTransparencyToggle : UIToggle
{
    [SerializeField]
    private Material transparentMaterial, normalMaterial;


    protected override void ToggleAction(bool active)
    {
        var buildingRenderers = RestrictionChecker.ActiveBuilding.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in buildingRenderers)
        {
            r.material = active ? transparentMaterial : normalMaterial;
        }
    }
}
