using System;
using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Cameras;
using Netherlands3D.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace T3D.Uitbouw
{
    public class WallSelectorSaveDataContainer : SaveDataContainer
    {
        public Vector3 RayOrigin;
        public Vector3 RayDirection;
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
        }

        private void OnWallDeselected(GameObject go)
        {
            WallMesh = new Mesh();
            wallMeshFilter.mesh = WallMesh;
            WallIsSelected = false;
            WallPlane = new Plane();
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
            Debug.LogError("todo: load wall");
        }
    }
}
