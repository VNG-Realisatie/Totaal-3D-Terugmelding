using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Netherlands3D.Events;
using T3D.Uitbouw;
using UnityEngine;
using UnityEngine.UI;
using WebGLFileUploaderExample;

public class SelectOptionState : State
{
    [SerializeField]
    private Toggle noModelToggle, drawToggle, uploadedModelToggle, snapToggle, freePlaceToggle, otherBuildingPartToggle;
    [SerializeField]
    private GameObject modelSettingsPanel, uploadPanel, notSupportedPanel;
    [SerializeField]
    private Button nextButton;
    private UploadedUitbouwVisualiser visualiser;
    [SerializeField]
    private GameObject visibilityPanel;

    [SerializeField]
    private TriggerEvent onUploadedModelVisualized;

    private bool modelLoaded;

    protected override void Awake()
    {
        base.Awake();
        visualiser = ServiceLocator.GetService<UploadedUitbouwVisualiser>();
    }

    private void OnEnable()
    {
        onUploadedModelVisualized.started.AddListener(OnUploadedModelVisualized);
    }

    private void OnDisable()
    {
        onUploadedModelVisualized.started.RemoveListener(OnUploadedModelVisualized);
    }

    void OnUploadedModelVisualized()
    {
        modelLoaded = true;
    }

    private void Update()
    {
        modelSettingsPanel.SetActive(!noModelToggle.isOn);
        notSupportedPanel.SetActive(otherBuildingPartToggle.isOn);
        uploadPanel.SetActive(uploadedModelToggle.isOn);
        if (noModelToggle.isOn)
            nextButton.interactable = true;
        else if (uploadedModelToggle.isOn)
            nextButton.interactable = !otherBuildingPartToggle.isOn && modelLoaded && !GetComponentInChildren<UploadModel>().IsLoading;
        else
            nextButton.interactable = !otherBuildingPartToggle.isOn;
    }

    public override int GetDesiredStateIndex()
    {
        ServiceLocator.GetService<T3DInit>().HTMLData.Add3DModel = !noModelToggle.isOn;
        ServiceLocator.GetService<T3DInit>().HTMLData.SnapToWall = snapToggle.isOn;
        ServiceLocator.GetService<T3DInit>().HTMLData.HasFile = uploadedModelToggle.isOn;

        if (noModelToggle.isOn)
        {
            return 0;
        }
        else if (snapToggle.isOn)
            return 1;
        else
            return 2;
    }

    protected override void LoadSavedState()
    {
        LoadModelToggles();

        uploadedModelToggle.isOn = ServiceLocator.GetService<T3DInit>().HTMLData.HasFile;

        StateLoadedAction();
        var stateSaver = GetComponentInParent<StateSaver>();
        var savedState = stateSaver.GetState(stateSaver.ActiveStateIndex);
        if (ActiveState != savedState)
        {
            //print("continue to next state");
            //if (uploadedModelToggle.isOn)
            //    LoadModel();
            //StartCoroutine(LoadModelAndGoToNextState());
            LoadModelAndGoToNextState();
        }
    }

    private void LoadModelToggles()
    {
        if (ServiceLocator.GetService<T3DInit>().HTMLData.Add3DModel && ServiceLocator.GetService<T3DInit>().HTMLData.HasFile)
        {
            uploadedModelToggle.isOn = true;
            LoadSnapToggles();
        }
        else if (ServiceLocator.GetService<T3DInit>().HTMLData.Add3DModel && !ServiceLocator.GetService<T3DInit>().HTMLData.HasFile)
        {
            drawToggle.isOn = true;
            LoadSnapToggles();
        }
        else
        {
            noModelToggle.isOn = true;
        }
    }

    private void LoadSnapToggles()
    {
        if (ServiceLocator.GetService<T3DInit>().HTMLData.SnapToWall)
            snapToggle.isOn = true;
        else
            freePlaceToggle.isOn = true;
    }

    public void LoadModelAndGoToNextState()
    {
        //if (ServiceLocator.GetService<T3DInit>().HTMLData.Add3DModel && ServiceLocator.GetService<T3DInit>().HTMLData.HasFile)
        if (ServiceLocator.GetService<T3DInit>().HTMLData.HasFile)
        {
            StartCoroutine(GetAndParseCityJson());
        }
        else
        {
            EndState();
        }
    }

    IEnumerator GetAndParseCityJson()
    {
        CoString result = new CoString();
        CoBool success = new CoBool();
        yield return StartCoroutine(UploadBimUtility.GetBimCityJson(result, success));

        //The CityJson has been downloaded, now lets visualize it
        if (success)
        {
            //Debug.Log("-------BimCityJsonReceived");
            ServiceLocator.GetService<Events>().RaiseBimCityJsonReceived(result);
            EndState();
        }
        else
        {
            ErrorService.GoToErrorPage(result);
        }
    }

    public override void StateEnteredAction()
    {
        base.StateEnteredAction();
        RestrictionChecker.ActiveBuilding.SelectedWall.gameObject.SetActive(false);
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
        base.StateCompletedAction();
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
