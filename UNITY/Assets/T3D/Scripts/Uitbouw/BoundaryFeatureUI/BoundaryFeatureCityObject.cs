using System.Collections;
using System.Collections.Generic;
using T3D.Uitbouw;
using T3D.Uitbouw.BoundaryFeatures;
using UnityEngine;

[RequireComponent(typeof(BoundaryFeature))]
public class BoundaryFeatureCityObject : CityObject
{
    public override CitySurface[] GetSurfaces()
    {
        var bf = GetComponent<BoundaryFeature>();

        SetParents(new CityObject[] {
            RestrictionChecker.ActiveUitbouw.GetComponent<CityObject>()
            });

        return new CitySurface[] { bf.Surface };
    }
}
