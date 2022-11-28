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
        private List<CityObject> cityObjects = new List<CityObject>();
        [SerializeField]
        private MeshFilter meshFilter;
        //public MeshFilter MeshFilter => meshFilter;
        //private Mesh mesh;

        //private MeshFilter tempMeshFilter;

        List<CityObjectVisualizer> uploadedCityObjectVisualizers = new List<CityObjectVisualizer>();

        //Vector3 transformedExtents;

        //private Vector3 localCenterOfAllmeshes;
        private Bounds combinedMeshBounds;

        public override Vector3 LeftCenter => meshFilter.transform.position + transform.rotation * combinedMeshBounds.center - transform.right * combinedMeshBounds.extents.x;

        public override Vector3 RightCenter => meshFilter.transform.position + transform.rotation * combinedMeshBounds.center + transform.right * combinedMeshBounds.extents.x;

        public override Vector3 TopCenter => meshFilter.transform.position + transform.rotation * combinedMeshBounds.center + transform.up * combinedMeshBounds.extents.y;

        public override Vector3 BottomCenter => meshFilter.transform.position + transform.rotation * combinedMeshBounds.center - transform.up * combinedMeshBounds.extents.y;

        public override Vector3 FrontCenter => meshFilter.transform.position + transform.rotation * combinedMeshBounds.center - transform.forward * combinedMeshBounds.extents.z;

        public override Vector3 BackCenter => meshFilter.transform.position + transform.rotation * combinedMeshBounds.center + transform.forward * combinedMeshBounds.extents.z;

        public override void UpdateDimensions()
        {
            //SetDimensions(Multiply(meshFilter.transform.lossyScale, mesh.bounds.size));
            var w = Vector3.Distance(LeftCenter, RightCenter);
            var d = Vector3.Distance(FrontCenter, BackCenter);
            var h = Vector3.Distance(BottomCenter, TopCenter);
            SetDimensions(w, d, h);
        }

        public void SetCombinedMesh(Mesh mesh)
        {
            //meshFilter = mf;
            print("setting mesh");
            //this.mesh = mesh;
            uploadedCityObjectVisualizers = GetComponentsInChildren<CityObjectVisualizer>().ToList();
            //var localCenterOfAllmeshes = new Vector3();
            //var minExtents = new Vector3();
            //var maxExtents = new Vector3();
            List<Vector3> allTransformedExtents = new List<Vector3>();
            //List<Vector3> transformedMaxExtents = new List<Vector3>();
            foreach (var v in uploadedCityObjectVisualizers)
            {
                //localCenterOfAllmeshes += v.transform.localPosition;
                //var min = v.transform.localPosition + v.ActiveMesh.bounds.min;
                //var max = v.transform.localPosition + v.ActiveMesh.bounds.max;
                allTransformedExtents.Add(v.transform.localPosition + v.ActiveMesh.bounds.min);
                allTransformedExtents.Add(v.transform.localPosition + v.ActiveMesh.bounds.max);
            }
            //localCenterOfAllmeshes /= uploadedCityObjectVisualizers.Count;

            var minX = allTransformedExtents.Min(e => e.x);
            var minY = allTransformedExtents.Min(e => e.y);
            var minZ = allTransformedExtents.Min(e => e.z);

            var maxX = allTransformedExtents.Max(e => e.x);
            var maxY = allTransformedExtents.Max(e => e.y);
            var maxZ = allTransformedExtents.Max(e => e.z);
            var localCenterOfAllmeshes = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
            var size = Multiply(meshFilter.transform.lossyScale, new Vector3(maxX - minX, maxY - minY, maxZ - minZ));
            combinedMeshBounds = new Bounds(localCenterOfAllmeshes, size);
            //transformedExtents = Multiply(meshFilter.transform.lossyScale, bounds.extents);
            UpdateDimensions();

            var offset = combinedMeshBounds.center;
            offset.y -= combinedMeshBounds.extents.y;
            offset += transform.forward * Depth / 2;

            foreach (var m in uploadedCityObjectVisualizers)
                m.transform.localPosition = -offset;

            combinedMeshBounds.center -= offset;

            //GetComponentInChildren<MeshCollider>().sharedMesh = meshFilter.mesh;
        }

        public static Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        //public void AddCityObject(CityJSONToCityObject newCityObject)
        //{
        //    if (cityObjects.Contains(newCityObject))
        //    {
        //        Debug.LogError("list already contains this City object");
        //        return;
        //    }

        //    cityObjects.Add(newCityObject);
        //    newCityObject.Type = CityObjectType.BuildingPart;
        //}

        public void UnparentFromMainBuilding()
        {
            foreach (var co in cityObjects)
            {
                co.SetParents(new CityObject[0]);
                co.Type = CityObjectType.Building;
            }
        }

        public void ReparentToMainBuilding(CityObject mainBuilding)
        {
            foreach (var co in cityObjects)
            {
                co.SetParents(new CityObject[] { mainBuilding });
                co.Type = CityObjectType.BuildingPart;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(LeftCenter, 0.1f);
            Gizmos.DrawSphere(RightCenter, 0.1f);
            Gizmos.DrawSphere(TopCenter, 0.1f);
            Gizmos.DrawSphere(BottomCenter, 0.1f);
            Gizmos.DrawSphere(FrontCenter, 0.1f);
            Gizmos.DrawSphere(BackCenter, 0.1f);
        }
    }
}