using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using Netherlands3D.T3DPipeline;
using System.Linq;

namespace T3D.Uitbouw
{
    public class UploadedUitbouw : UitbouwBase
    {
        [SerializeField]
        private CityJSON cityJSON;

        private Bounds combinedMeshBounds;

        public override Vector3 LeftCenter => transform.position + transform.rotation * combinedMeshBounds.center - transform.right * combinedMeshBounds.extents.x;

        public override Vector3 RightCenter => transform.position + transform.rotation * combinedMeshBounds.center + transform.right * combinedMeshBounds.extents.x;

        public override Vector3 TopCenter => transform.position + transform.rotation * combinedMeshBounds.center + transform.up * combinedMeshBounds.extents.y;

        public override Vector3 BottomCenter => transform.position + transform.rotation * combinedMeshBounds.center - transform.up * combinedMeshBounds.extents.y;

        public override Vector3 FrontCenter => transform.position + transform.rotation * combinedMeshBounds.center - transform.forward * combinedMeshBounds.extents.z;

        public override Vector3 BackCenter => transform.position + transform.rotation * combinedMeshBounds.center + transform.forward * combinedMeshBounds.extents.z;

        public override void UpdateDimensions()
        {
            var w = Vector3.Distance(LeftCenter, RightCenter);
            var d = Vector3.Distance(FrontCenter, BackCenter);
            var h = Vector3.Distance(BottomCenter, TopCenter);
            SetDimensions(w, d, h);
        }

        public void RecalculateBounds(CityObject[] cityObjects)
        {
            List<Vector3> allTransformedExtents = new List<Vector3>();
            foreach (var co in cityObjects)
            {
                var v = co.GetComponent<CityObjectVisualizer>();
                allTransformedExtents.Add(v.transform.localPosition + v.ActiveMesh.bounds.min);
                allTransformedExtents.Add(v.transform.localPosition + v.ActiveMesh.bounds.max);
            }

            var minX = allTransformedExtents.Min(e => e.x);
            var minY = allTransformedExtents.Min(e => e.y);
            var minZ = allTransformedExtents.Min(e => e.z);

            var maxX = allTransformedExtents.Max(e => e.x);
            var maxY = allTransformedExtents.Max(e => e.y);
            var maxZ = allTransformedExtents.Max(e => e.z);
            var localCenterOfAllmeshes = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
            var size = Multiply(transform.lossyScale, new Vector3(maxX - minX, maxY - minY, maxZ - minZ));
            combinedMeshBounds = new Bounds(localCenterOfAllmeshes, size);

            UpdateDimensions();
        }

        public void SetMeshOffset()
        {
            var offset = -combinedMeshBounds.center;
            offset.y += combinedMeshBounds.extents.y;
            offset -= Vector3.forward * Depth / 2;

            var uploadedCityObjectVisualizers = GetComponentsInChildren<CityObjectVisualizer>();
            foreach (var m in uploadedCityObjectVisualizers)
            {
                m.transform.localPosition += offset;
            }

            combinedMeshBounds.center += offset;
        }

        public static Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public void UnparentFromMainBuilding()
        {
            var uitbouwCityObjects = cityJSON.CityObjects;
            //todo: now we take the first as the building, because there is currently no way to determine which of them is the Main building
            var mainBuilding = uitbouwCityObjects[0];
            mainBuilding.Type = CityObjectType.Building;
            var mainBuildingId = RestrictionChecker.ActiveBuilding.MainCityObject.Id;
            mainBuilding.SetId(mainBuildingId);

            var existingChildren = mainBuilding.CityChildren.Length;
            for (int i = 1; i < uitbouwCityObjects.Length; i++) //skip the first
            {
                CityObject co = uitbouwCityObjects[i];
                var newId = mainBuilding.Id + "-" + (existingChildren + i).ToString();
                co.SetId(newId);
                co.Type = CityObjectType.BuildingPart;
                co.SetParents(new CityObject[] { mainBuilding });
            }
        }

        public void ReparentToMainBuilding(CityObject mainBuilding)
        {
            var uitbouwCityObjects = cityJSON.CityObjects;
            var existingChildren = mainBuilding.CityChildren.Length;
            for (int i = 0; i < uitbouwCityObjects.Length; i++)
            {
                CityObject co = uitbouwCityObjects[i];
                var newId = mainBuilding.Id + "-" + (existingChildren + i).ToString();
                co.SetId(newId);
                co.Type = CityObjectType.BuildingPart;
                co.SetParents(new CityObject[] { mainBuilding });
            }
        }
    }
}