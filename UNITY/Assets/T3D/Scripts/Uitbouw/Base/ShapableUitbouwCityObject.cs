using System.Collections;
using System.Collections.Generic;
using T3D.Uitbouw.BoundaryFeatures;
using T3D.Uitbouw;
using UnityEngine;
using Netherlands3D.T3DPipeline;

namespace T3D.Uitbouw
{
    [RequireComponent(typeof(UitbouwBase))]
    public class ShapableUitbouwCityObject : CityObject
    {
        protected void Start()
        {
            //base.Start();
            var building = GetComponent<UitbouwBase>().ActiveBuilding;
            Debug.LogError("set parents here after finishing converting to T3DPipeline");
            //SetParents(new CityObject[] {
            //    building.MainCityObject
            //});
        }

        //todo
        //public override CitySurface[] GetSurfaces()
        //{
        //    List<CitySurface> citySurfaces = new List<CitySurface>();
        //    var walls = GetComponentsInChildren<UitbouwMuur>();
        //    foreach (var wall in walls)
        //    {
        //        citySurfaces.Add(wall.Surface);
        //    }

        //    var boundaryFeatures = GetComponentsInChildren<BoundaryFeature>();
        //    foreach (var bf in boundaryFeatures)
        //    {
        //        citySurfaces.Add(bf.Surface);
        //    }

        //    return citySurfaces.ToArray();
        //}
    }
}