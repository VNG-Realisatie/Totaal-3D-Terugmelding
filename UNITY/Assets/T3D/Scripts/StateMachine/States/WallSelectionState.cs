﻿using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Cameras;
using T3D;
using T3D.Uitbouw;
using UnityEngine;
using UnityEngine.UI;

public class WallSelectionState : State
{
    private BuildingMeshGenerator building;

    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Text infoText;

    [SerializeField]
    private string selectedText = "aangegeven gevel";
    [SerializeField]
    private string unselectedText = "selecteer gevel";

    [SerializeField]
    private GameObject visibilityPanel;

    protected override void Awake()
    {
        base.Awake();
        building = RestrictionChecker.ActiveBuilding;
        building.SelectedWall.AllowSelection = true;

        nextButton.interactable = false;
    }

    private void Update()
    {
        ProcessUIState();
    }

    private void ProcessUIState()
    {
        if (LocationIsSelected())
        {
            nextButton.interactable = true;
            infoText.text = selectedText;
        }
        else
        {
            nextButton.interactable = false;
            infoText.text = unselectedText;
        }
    }

    private bool LocationIsSelected()
    {
        return building.SelectedWall.WallIsSelected;
    }

    public override int GetDesiredStateIndex()
    {
        if (ServiceLocator.GetService<T3DInit>().HTMLData == null)
            desiredNextStateIndex = 0;
        else
            desiredNextStateIndex = ServiceLocator.GetService<T3DInit>().HTMLData.HasFile ? 0 : 1;
        return desiredNextStateIndex;
    }

    public override void StateEnteredAction()
    {
        ServiceLocator.GetService<CameraModeChanger>().SetCameraMode(CameraMode.GodView);

        building.SelectedWall.gameObject.SetActive(true);
        building.SelectedWall.AllowSelection = true;
        building.SelectedWall.WallChanged = false;

        ServiceLocator.GetService<MetadataLoader>().EnableActiveuitbouw(false);
        foreach (var uiToggle in visibilityPanel.GetComponentsInChildren<UIToggle>(true))
        {
            if (uiToggle as DisableMainBuildingToggle)
            {
                uiToggle.SetIsOn(true);
                uiToggle.SetVisible(false);
                continue;
            }

            if (uiToggle as DisableUitbouwToggle)
            {
                uiToggle.SetIsOn(true);
                uiToggle.SetVisible(false);
                continue;
            }
        }
    }

    public override void StateCompletedAction()
    {
        building.SelectedWall.AllowSelection = false;

        foreach (var uiToggle in visibilityPanel.GetComponentsInChildren<UIToggle>(true))
        {
            if (uiToggle as DisableMainBuildingToggle)
            {
                var uploadedUitbouw = ServiceLocator.GetService<T3DInit>().HTMLData.HasFile;
                var drawChange = ServiceLocator.GetService<T3DInit>().HTMLData.Add3DModel;
                var snapToWall = ServiceLocator.GetService<T3DInit>().HTMLData.SnapToWall;
                uiToggle.SetVisible(uploadedUitbouw && drawChange && !snapToWall);
                continue;
            }

            if (uiToggle as DisableUitbouwToggle)
            {
                var drawChange = ServiceLocator.GetService<T3DInit>().HTMLData.Add3DModel;
                uiToggle.SetVisible(drawChange);
                continue;
            }
        }
    }
}
