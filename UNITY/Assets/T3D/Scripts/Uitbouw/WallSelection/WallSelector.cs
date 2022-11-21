using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Netherlands3D.Cameras;
using Netherlands3D.Events;
using Netherlands3D.T3DPipeline;
using UnityEngine;
using UnityEngine.EventSystems;

namespace T3D.Uitbouw
{
    public class WallSelectorSaveDataContainer : SaveDataContainer
    {
        public string CityObjectId;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
    }

    public class WallSelector : MonoBehaviour
    {
        private MeshFilter wallMeshFilter;

        private BuildingMeshGenerator building;

        [SerializeField]
        private GameObjectEvent onWallSelected;
        [SerializeField]
        private GameObjectEvent onWallDeselected;

        private bool allowSelection;
        public bool AllowSelection
        {
            get
            {
                return allowSelection;
            }
            set
            {
                allowSelection = value;
                foreach (var selector in building.GetComponentsInChildren<CityObjectWallSelector>())
                {
                    selector.AllowSelection = value;
                }
            }
        }

        public bool WallIsSelected { get; private set; }
        public Plane WallPlane { get; private set; }
        public Mesh WallMesh { get; private set; }
        public Vector3 CenterPoint { get; private set; }
        public bool WallChanged { get; set; } //is a different wall selected than before? Used to reposition the uitbouw when user went back and selected a different wall

        private WallSelectorSaveDataContainer saveData;

        private void Awake()
        {
            saveData = new WallSelectorSaveDataContainer();

            wallMeshFilter = GetComponent<MeshFilter>();
            building = GetComponentInParent<BuildingMeshGenerator>();
        }

        private void OnEnable()
        {
            building.BuildingDataProcessed += Building_BuildingDataProcessed;
            onWallSelected.started.AddListener(OnWallSelected);
            onWallDeselected.started.AddListener(OnWallDeselected);
        }

        private void OnDisable()
        {
            building.BuildingDataProcessed -= Building_BuildingDataProcessed;
            onWallSelected.started.RemoveListener(OnWallSelected);
            onWallDeselected.started.RemoveListener(OnWallDeselected);
        }

        private void OnWallSelected(GameObject go)
        {
            var wallSelector = go.GetComponent<CityObjectWallSelector>();
            if (!wallSelector.CheckIfGrounded(building.GroundLevel))
            {
                OnWallDeselected(go);
                return;
            }
            transform.position = go.transform.position;
            WallMesh = wallSelector.WallMesh;
            wallMeshFilter.mesh = WallMesh;
            WallIsSelected = true;
            WallChanged = true;

            saveData.CityObjectId = go.GetComponent<CityObject>().Id;
            saveData.HitPoint = wallSelector.HitPoint;
            saveData.HitNormal = wallSelector.HitNormal;
        }

        private void OnWallDeselected(GameObject go)
        {
            WallMesh = new Mesh();
            wallMeshFilter.mesh = WallMesh;
            WallIsSelected = false;
            WallPlane = new Plane();

            saveData.CityObjectId = string.Empty;
            saveData.HitPoint = Vector3.zero;
            saveData.HitNormal = Vector3.zero;
        }

        private void Building_BuildingDataProcessed(BuildingMeshGenerator building)
        {
            if (SessionSaver.LoadPreviousSession)
            {
                LoadSelectedWall();
            }
        }

        private void LoadSelectedWall()
        {
            var cityObjects = GetComponentInParent<CityJSON>().CityObjects;
            var selectedWallObject = cityObjects.FirstOrDefault(co => co.Id == saveData.CityObjectId);

            if (selectedWallObject)
                selectedWallObject.GetComponent<CityObjectWallSelector>().ProcessHit(saveData.HitPoint, saveData.HitNormal);
        }
    }
}
