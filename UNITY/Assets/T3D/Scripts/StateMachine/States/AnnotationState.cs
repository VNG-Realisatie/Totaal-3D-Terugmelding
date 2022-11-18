using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Netherlands3D.Events;
using Netherlands3D.Interface;
using Netherlands3D.T3DPipeline;
using T3D.Uitbouw;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationState : State
{
    //[SerializeField]
    //private AnnotationMarker markerPrefab;
    //List<AnnotationMarker> annotationMarkers = new List<AnnotationMarker>();

    [SerializeField]
    private RectTransform annotationParent;
    [SerializeField]
    private AnnotationUI annotationPrefab;
    [SerializeField]
    private ScrollToSelected scroll;

    public GameObject PreviousButton;

    public static List<AnnotationUI> AnnotationUIs = new List<AnnotationUI>();
    public int AmountOfAnnotations => AnnotationUIs.Count;

    //public bool AllowSelection { get; private set; } = true;
    public int ActiveSelectedId = 0;

    //[SerializeField]
    //private Vector3Event buildingClicked; //use to load data and invoke clicks at those points
    [SerializeField]
    private IntEvent onAnnotationStarted;
    [SerializeField]
    private TriggerEvent annotationCompleted;
    [SerializeField]
    private IntEvent reselectAnnotationEvent;

    //Dictionary<int, string> savedAnnotationTexts = new Dictionary<int, string>();
    //Dictionary<int, StringIntPair> globalToLocalIds = new Dictionary<int, StringIntPair>();

    protected override void Awake()
    {
        base.Awake();
        AnnotationUIs = new List<AnnotationUI>(); //ensure the static list is emptied whenever the scene is reset
    }

    private void OnEnable()
    {
        onAnnotationStarted.started.AddListener(OnAnnotationStarted);
        reselectAnnotationEvent.started.AddListener(SelectAnnotation);
    }

    private void OnDisable()
    {
        onAnnotationStarted.started.RemoveListener(OnAnnotationStarted);
        reselectAnnotationEvent.started.RemoveListener(SelectAnnotation);
    }

    public override void GoToPreviousState()
    {
        base.GoToPreviousState();
        DisplayAnnotations(false);
    }

    public override int GetDesiredStateIndex()
    {
        return 3;
        //if (ServiceLocator.GetService<T3DInit>().HTMLData == null)
        //    return 0;

        if (ServiceLocator.GetService<T3DInit>().HTMLData.Add3DModel == false) return 2;

        return ServiceLocator.GetService<T3DInit>().HTMLData.SnapToWall ? 0 : 1;
    }

    public override void StateLoadedAction()
    {
        if (SessionSaver.LoadPreviousSession)
            LoadSavedAnnotations();
    }

    private void LoadSavedAnnotations() //todo
    {
        var annotationSaveDataNode = SessionSaver.GetJSONNodeOfType(typeof(AnnotationUISaveData).ToString());

        //onAnnotationStarted.started.AddListener(OnLoadAnnotationStarted);
        foreach (var node in annotationSaveDataNode)
        {
            var data = node.Value;

            var parent = data["ParentCityObject"];
            var connectionPoint = data["ConnectionPoint"];
            var text = data["AnnotationText"];

            var coas = RestrictionChecker.ActiveBuilding.GetComponentsInChildren<CityObjectAnnotations>();

            foreach (var coa in coas)
            {
                if (coa.CityObject.Id != parent)
                    continue;

                coa.StartAddNewAnnotation(connectionPoint); //this will invoke events and create uis for use on the next line
                int globalId = AnnotationUIs.Count - 1;
                reselectAnnotationEvent.Invoke(globalId);
                AnnotationUIs[globalId].SetText(text);
            }
        }
    }

    public override void StateEnteredAction()
    {
        base.StateEnteredAction();
        if (!SessionSaver.LoadPreviousSession)
            RemoveAllAnnotations();

        DisplayAnnotations(true);

        foreach (var coa in RestrictionChecker.ActiveBuilding.GetComponentsInChildren<CityObjectAnnotations>())
        {
            coa.AnnotationStateActive = true;
        }
    }

    public override void StateCompletedAction()
    {
        foreach (var coa in RestrictionChecker.ActiveBuilding.GetComponentsInChildren<CityObjectAnnotations>())
        {
            coa.AnnotationStateActive = false;
        }
    }

    protected override void Start()
    {
        base.Start();
        RecalculeteContentHeight();
    }

    void Update()
    {
        var maskMarker = LayerMask.GetMask("SelectionPoints");
        if (T3D.ObjectClickHandler.GetClickOnObject(false, out var hit, maskMarker, false))
        {
            //SelectAnnotation(hit.collider.GetComponent<T3D.Uitbouw.AnnotationMarker>().Id);
            var id = hit.collider.GetComponent<T3D.Uitbouw.AnnotationMarker>().Id;
            reselectAnnotationEvent.Invoke(id);
        }
    }

    private void CreateAnnotationUI(string parentCityObject, Vector3 connectionPoint, int globalId)
    {
        var annotationUI = Instantiate(annotationPrefab, annotationParent);
        AnnotationUIs.Add(annotationUI);
        annotationUI.Initialize(globalId, parentCityObject, connectionPoint);
        RecalculeteContentHeight();
    }

    private void OnAnnotationStarted(int globalId)
    {
        annotationCompleted.Invoke(); //complete annotation so it is added to the list
        var ann = CityObjectAnnotations.GetAnnotation(globalId);
        CreateAnnotationUI(ann.ParentAttribute.ParentCityObject.Id, (Vector3)ann.Position, globalId);
        AnnotationUIs[globalId].SetId(globalId);
        var marker = ann.AnnotationMarker.GetComponent<T3D.Uitbouw.AnnotationMarker>();
        marker.SetId(globalId);

        SelectAnnotation(globalId); //reselect completed annotation
    }

    private void SelectAnnotation(int globalId)
    {
        var oldAnnotation = CityObjectAnnotations.GetAnnotation(ActiveSelectedId);
        AnnotationUIs[ActiveSelectedId].SetSelectedColor(false);
        var marker = oldAnnotation.AnnotationMarker.GetComponent<T3D.Uitbouw.AnnotationMarker>();
        marker.SetSelectedColor(false);

        var newSelectedAnnotationUI = AnnotationUIs[globalId];
        var newAnnotation = CityObjectAnnotations.GetAnnotation(globalId);
        var newSelectedMarker = newAnnotation.AnnotationMarker.GetComponent<T3D.Uitbouw.AnnotationMarker>();

        if (!newSelectedAnnotationUI.IsOpen)
            newSelectedAnnotationUI.ToggleAnnotation();

        newSelectedAnnotationUI.SetSelectedColor(true);
        newSelectedMarker.SetSelectedColor(true);

        RecalculeteContentHeight();
        scroll.SetSelectedChild(globalId);

        ActiveSelectedId = globalId;
    }

    public void RemoveAllAnnotations()
    {
        for (int i = AnnotationUIs.Count - 1; i >= 0; i--)//go backwards to avoid collection modified errors
        {
            RemoveAnnotation(i);
        }
    }

    public void RemoveAnnotation(int globalId)
    {
        var ui = AnnotationUIs[globalId];
        AnnotationUIs.Remove(ui);
        Destroy(ui.gameObject);
        CityObjectAnnotations.DeleteAnnotationWithGlobalId(globalId);

        if (globalId == ActiveSelectedId)
            ActiveSelectedId = 0;
        else if (ActiveSelectedId > globalId)
            ActiveSelectedId--;

        RecalculateAnnotationUIIds();
        RecalculeteContentHeight();
    }

    private void RecalculateAnnotationUIIds()
    {
        for (int i = 0; i < AnnotationUIs.Count; i++)
        {
            AnnotationUIs[i].SetId(i);
            var ann = CityObjectAnnotations.GetAnnotation(i);
            var marker = ann.AnnotationMarker.GetComponent<T3D.Uitbouw.AnnotationMarker>();
            marker.SetId(i);
        }
    }

    public void RecalculeteContentHeight()
    {
        float height = 35f; //top and bottom margin

        foreach (var ann in AnnotationUIs)
        {
            height += ann.GetComponent<RectTransform>().sizeDelta.y;
        }
        annotationParent.sizeDelta = new Vector2(annotationParent.sizeDelta.x, AnnotationUIs.Count > 0 ? height : 0);
    }

    public void DisplayAnnotations(bool show)
    {
        for (int i = 0; i < AnnotationUIs.Count; i++)
        {
            var ann = CityObjectAnnotations.GetAnnotation(i);
            ann.AnnotationMarker.GetComponent<T3D.Uitbouw.AnnotationMarker>().ShowMarker(show);
        }
    }
}
