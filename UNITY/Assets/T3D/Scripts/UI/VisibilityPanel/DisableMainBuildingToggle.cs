using System.Collections;
using System.Collections.Generic;
using Netherlands3D.T3DPipeline;
using T3D.Uitbouw;
using UnityEngine;
using UnityEngine.UI;

public class DisableMainBuildingToggle : UIToggle
{
    private Dictionary<CityObject, int> activeLods = new Dictionary<CityObject, int>();

    private void Start()
    {
        SetVisible(false);
    }

    protected override void ToggleAction(bool active)
    {
        var buildings = RestrictionChecker.ActiveBuilding.GetComponent<CityJSON>().CityObjects;
        var uitbouw = RestrictionChecker.ActiveUitbouw as UploadedUitbouw;//.GetComponent<CityObject>();
        if (active)
        {
            foreach (var co in buildings)
            {
                co.GetComponent<CityObjectVisualizer>().SetLODActive(activeLods[co]);
                CityJSONFormatter.AddCityObject(co);
                uitbouw.ReparentToMainBuilding(RestrictionChecker.ActiveBuilding.MainCityObject);
            }
            var snapToWall = ServiceLocator.GetService<T3DInit>().HTMLData.SnapToWall;
            RestrictionChecker.ActiveBuilding.SelectedWall.gameObject.SetActive(snapToWall);
            //uitbouw.Type = uitbouwType;
        }
        else
        {
            //save data to set back when toggle is turned on again
            activeLods = new Dictionary<CityObject, int>();
            //uitbouwType = uitbouw.Type;

            foreach (var co in buildings)
            {
                activeLods.Add(co, co.GetComponent<CityObjectVisualizer>().ActiveLod);
                co.GetComponent<CityObjectVisualizer>().SetLODActive(-1);
                CityJSONFormatter.RemoveCityObject(co);
            }
            RestrictionChecker.ActiveBuilding.SelectedWall.gameObject.SetActive(false);
            //uitbouw.Type = CityObjectType.Building;
            uitbouw.UnparentFromMainBuilding();
        }
    }
}
