using System.Collections;
using System.Collections.Generic;
using Netherlands3D.T3D.Uitbouw;
using T3D.Uitbouw;
using UnityEngine;
using UnityEngine.UI;

public class DisableMainBuildingToggle : UIToggle
{
    private Dictionary<CityJSONToCityObject, int> activeLods = new Dictionary<CityJSONToCityObject, int>();

    private void Start()
    {
        SetVisible(false);
    }

    protected override void ToggleAction(bool active)
    {
        var buildings = RestrictionChecker.ActiveBuilding.GetComponentsInChildren<CityJSONToCityObject>();
        var uitbouw = RestrictionChecker.ActiveUitbouw as UploadedUitbouw;//.GetComponent<CityObject>();
        if (active)
        {
            foreach (var co in buildings)
            {
                co.SetMeshActive(activeLods[co]);
                CityJSONFormatter.AddCityObejct(co);
                uitbouw.ReparentToMainBuilding(RestrictionChecker.ActiveBuilding.MainCityObject);
            }
            var snapToWall = ServiceLocator.GetService<T3DInit>().HTMLData.SnapToWall;
            RestrictionChecker.ActiveBuilding.SelectedWall.gameObject.SetActive(snapToWall);
            //uitbouw.Type = uitbouwType;
        }
        else
        {
            //save data to set back when toggle is turned on again
            activeLods = new Dictionary<CityJSONToCityObject, int>();
            //uitbouwType = uitbouw.Type;

            foreach (var co in buildings)
            {
                activeLods.Add(co, co.ActiveLod);
                co.SetMeshActive(-1);
                CityJSONFormatter.RemoveCityObject(co);
            }
            RestrictionChecker.ActiveBuilding.SelectedWall.gameObject.SetActive(false);
            //uitbouw.Type = CityObjectType.Building;
            uitbouw.UnparentFromMainBuilding();
        }
    }
}
