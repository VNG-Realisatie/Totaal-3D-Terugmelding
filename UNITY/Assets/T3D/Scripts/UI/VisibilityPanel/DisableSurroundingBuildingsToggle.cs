using System.Collections;
using System.Collections.Generic;
using T3D.Uitbouw;
using UnityEngine;
using UnityEngine.UI;

public class DisableSurroundingBuildingsToggle : UIToggle
{
    public GameObject BuildingsLayer;

    protected override void ToggleAction(bool active)
    {
        BuildingsLayer.SetActive(active);
        RestrictionChecker.ActivePerceel.SetTerrainActive(active);
        RestrictionChecker.ActivePerceel.SetPerceelActive(!active);
    }
}
