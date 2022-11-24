using System.Collections;
using System.Collections.Generic;
using T3D.Uitbouw.BoundaryFeatures;
using T3D.Uitbouw;
using UnityEngine;
using Netherlands3D.T3DPipeline;
using System;
using UnityEngine.Assertions;
using System.Linq;
using Netherlands3D.Core;

namespace T3D.Uitbouw
{
    [RequireComponent(typeof(UitbouwBase))]
    public class ShapableUitbouwCityObject : CityObject
    {
        CityMultiOrCompositeSurface multiSurface => Geometries[0].BoundaryObject as CityMultiOrCompositeSurface;

        protected void Awake()
        {
            var building = GetComponent<UitbouwBase>().ActiveBuilding;
            Initialize(building.MainCityObject);
            print(building.MainCityObject);
        }


        public void Initialize(CityObject parent)
        {
            Id = parent.Id + "-" + parent.CityChildren.Length;
            Type = CityObjectType.BuildingPart;
            CoordinateSystem = parent.CoordinateSystem;
            Geometries = new List<CityGeometry>();

            var geometry = new CityGeometry(GeometryType.MultiSurface, 3, true, false, false);
            Assert.IsTrue(CityGeometry.IsValidType(Type, geometry.Type));
            Geometries.Add(geometry);

            multiSurface.Surfaces = new List<Netherlands3D.T3DPipeline.CitySurface>(); //remove default surface

            SetParents(new CityObject[] { parent });
        }

        private void RecalculateExtents()
        {
            var verts = GetUncombinedGeometryVertices();
            if (verts.Count > 0)
            {
                var minX = verts.Min(v => v.x);
                var minY = verts.Min(v => v.y);
                var minZ = verts.Min(v => v.z);
                var maxX = verts.Max(v => v.x);
                var maxY = verts.Max(v => v.y);
                var maxZ = verts.Max(v => v.z);

                MinExtent = new Vector3Double(minX, minY, minZ);
                MaxExtent = new Vector3Double(maxX, maxY, maxZ);
            }
        }

        public void AddSurface(Netherlands3D.T3DPipeline.CitySurface surface)
        {
            multiSurface.Surfaces.Add(surface);
            RecalculateExtents();
        }

        public void RemoveSurface(Netherlands3D.T3DPipeline.CitySurface surface)
        {
            multiSurface.Surfaces.Remove(surface);
            RecalculateExtents();
        }
    }
}