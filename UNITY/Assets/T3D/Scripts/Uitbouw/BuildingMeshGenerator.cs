using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netherlands3D.Core;
using Netherlands3D.Cameras;
using Netherlands3D.T3DPipeline;
using Netherlands3D.Events;

namespace T3D.Uitbouw
{
    public class BuildingMeshGenerator : MonoBehaviour
    {
        public Vector3 BuildingCenter { get; private set; }
        public float GroundLevel { get; private set; }
        public float HeightLevel { get; private set; }
        public bool IsMonument { get; private set; }
        public bool IsBeschermd { get; private set; }
        public float Area { get; private set; }

        public WallSelector SelectedWall { get; private set; }

        //public Vector3[] RelativeBuildingCorners { get; private set; }
        public Vector3[] AbsoluteBuildingCorners { get; private set; }
        public Plane[] RoofEdgePlanes { get; private set; }
        private float roofEdgeYTolerance = 0.1f;

        public bool BuildingDataIsProcessed { get; private set; } = false;

        public delegate void BuildingDataProcessedEventHandler(BuildingMeshGenerator building);
        public event BuildingDataProcessedEventHandler BuildingDataProcessed;

        //private CityJsonModel cityJsonModel;
        public CityObject MainCityObject { get; private set; }

        public int ActiveLod = 2;
        [SerializeField]
        private DoubleArrayEvent positionReceived;

        private void Awake()
        {
            SelectedWall = GetComponentInChildren<WallSelector>();
        }

        protected void Start()//in start to avoid race conditions
        {
            ServiceLocator.GetService<MetadataLoader>().BuildingOutlineLoaded += Instance_BuildingOutlineLoaded;
            SessionSaver.Loader.LoadingCompleted += Loader_LoadingCompleted;
            positionReceived.started.AddListener(ProcessPosition);
        }

        //called by event in the inspector
        public void ProcessPosition(double[] position)
        {
            var pos = ServiceLocator.GetService<T3DInit>().HTMLData.RDPosition;
            var bagId = ServiceLocator.GetService<T3DInit>().HTMLData.BagId;
            GotoPosition(pos);
            ServiceLocator.GetService<MetadataLoader>().RequestPerceelAndBuildingOutlineData(pos, bagId);
        }

        //called by event in the inspector
        public void ProcessBuilding()
        {
            var cityObjects = GetComponent<CityJSON>().CityObjects;
            MainCityObject = cityObjects.FirstOrDefault(co => co.Type == CityObjectType.Building);
            foreach (var co in cityObjects)
            {
                co.gameObject.layer = LayerMask.NameToLayer("ActiveSelection");
            }

            ProcessActiveMesh();
        }

        private void ProcessActiveMesh()
        {
            //get all active lods.
            List<Mesh> activeMeshes = new List<Mesh>();
            Vector3 positionOffset = Vector3.zero;
            foreach (var co in GetComponent<CityJSON>().CityObjects)
            {
                var visualizer = co.GetComponent<CityObjectVisualizer>();
                var hasActiveMesh = visualizer.SetLODActive(ActiveLod);

                if (hasActiveMesh)
                {
                    var mesh = visualizer.GetComponent<MeshFilter>().mesh;
                    activeMeshes.Add(mesh);
                    positionOffset += co.transform.position;
                }
            }

            positionOffset /= activeMeshes.Count;
            var activeCO = GetComponent<CityJSON>().CityObjects.FirstOrDefault(co => co.Geometries.FirstOrDefault(g => g.Lod == ActiveLod) != null);

            var activeMesh = CityObjectVisualizer.CombineMeshes(activeMeshes, transform.localToWorldMatrix);

            if (activeMesh)
                ProcessMesh(activeMesh, positionOffset);
        }

        void GotoPosition(Vector3RD position)
        {
            Vector3 cameraOffsetForTargetLocation = new Vector3(0, 38, 0);
            ServiceLocator.GetService<CameraModeChanger>().ActiveCamera.transform.position = CoordConvert.RDtoUnity(position) + cameraOffsetForTargetLocation;
            ServiceLocator.GetService<CameraModeChanger>().ActiveCamera.transform.LookAt(CoordConvert.RDtoUnity(position), Vector3.up);
        }

        private void ProcessMesh(Mesh mesh, Vector3 positionOffset)
        {
            BuildingCenter = mesh.bounds.center + positionOffset;
            GroundLevel = BuildingCenter.y - mesh.bounds.extents.y; //hack: if the building geometry goes through the ground this will not work properly
            HeightLevel = BuildingCenter.y + mesh.bounds.extents.y;

            RoofEdgePlanes = ProcessRoofEdges(mesh, positionOffset.y);

            BuildingDataProcessed.Invoke(this); // it cannot be assumed if the perceel or building data loads + processes first due to the server requests, so this event is called to make sure the processed building information can be used by other classes
            BuildingDataIsProcessed = true;
        }

        public void ResetBuilding()
        {
            BuildingDataIsProcessed = false;
        }

        private void Loader_LoadingCompleted(bool loadSucceeded)
        {
            IsMonument = ServiceLocator.GetService<T3DInit>().HTMLData.IsMonument;
            IsBeschermd = ServiceLocator.GetService<T3DInit>().HTMLData.IsBeschermd;
        }

        private Plane[] ProcessRoofEdges(Mesh buildingMesh, float yOffset)
        {
            var verts = buildingMesh.vertices;
            List<Plane> planes = new List<Plane>();
            List<float> yValues = new List<float>();

            foreach (var vert in verts)
            {
                var y = vert.y + yOffset;
                if (!IsWithinAnyYToleranceRange(yValues, y, roofEdgeYTolerance))
                {
                    yValues.Add(y);
                    planes.Add(new Plane(-Vector3.up, y));
                }
            }

            return planes.ToArray();
        }

        private static bool IsWithinAnyYToleranceRange(List<float> yValues, float y, float tolerance)
        {
            var index = yValues.FindIndex(num => Mathf.Abs(num - y) < tolerance);
            return index != -1;
        }

        private void Instance_BuildingOutlineLoaded(object source, BuildingOutlineEventArgs args)
        {
            Area = args.TotalArea;
            StartCoroutine(ProcessCorners(args.Outline));
        }

        private IEnumerator ProcessCorners(List<Vector2[]> coords)
        {
            yield return new WaitUntil(() => BuildingDataIsProcessed); //wait until ground level is set

            var q = from i in coords
                    from p in i
                    select CoordConvert.RDtoUnity(p) into v3
                    select new Vector3(v3.x, GroundLevel, v3.z);

            AbsoluteBuildingCorners = q.ToArray();
        }
    }
}
