using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Netherlands3D.Cameras;
using Netherlands3D.T3D;
using Netherlands3D.T3D.Uitbouw;
using UnityEngine;

public struct MeasureLine
{
    public MeasurePoint Start;
    public MeasurePoint End;

    public bool ValidStartPoint;
    public bool ValidEndPoint;

    public MeasureLine(MeasurePoint start, MeasurePoint end, bool validStart, bool validEnd)
    {
        Start = start;
        End = end;

        ValidStartPoint = validStart;
        ValidEndPoint = validEnd;
    }
}

public class UitbouwFreeMeasurement : DistanceMeasurement
{
    private SelectableMesh[] mySelectableMeshes;
    private SelectableMesh[] otherSelectableMeshes;

    private List<MeasureLine> measureLines = new List<MeasureLine>();
    private MeasurePoint firstPoint;
    //private MeasurePoint secondPoint;
    private Vector3 mousePositionInWorld;

    private bool measureToolActive;
    private UitbouwBase uitbouw;

    [SerializeField]
    private MeasurePoint cursorPointPrefab;

    protected override void Awake()
    {
        numberOfLines = 0;
        lines = new List<BuildingMeasuring>();
        uitbouw = GetComponent<UitbouwBase>();
    }

    private void Start()
    {
        otherSelectableMeshes = new SelectableMesh[2];
        otherSelectableMeshes[0] = RestrictionChecker.ActivePerceel.GetComponentInChildren<SelectableMesh>();
        otherSelectableMeshes[1] = RestrictionChecker.ActiveBuilding.GetComponentInChildren<SelectableMesh>();
    }

    protected override void DrawLines()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            var start = measureLines[i].Start.transform.position;
            //print("end exists: " + (measureLines[i].End != null));
            var end = measureLines[i].End != null ? measureLines[i].End.transform.position : mousePositionInWorld;
            lines[i].SetLinePosition(start, end);
        }
    }

    protected override void Measuring_DistanceInputOverride(BuildingMeasuring source, Vector3 direction, float delta)
    {
        //flip direction if points selected in wrong order
        var index = lines.IndexOf(source);
        var lineMesh = measureLines[index].End.GetComponentInParent<SelectableMesh>();
        if (mySelectableMeshes.Contains(lineMesh))
            delta *= -1;
        uitbouw.GetComponent<UitbouwMovement>().SetPosition(uitbouw.transform.position + direction * delta);
    }

    protected override void Update()
    {
        base.Update();

        if (!measureToolActive && uitbouw.Gizmo.Mode == GizmoMode.Measure)
            SetAllowMeasurement(true);
        else if (measureToolActive && uitbouw.Gizmo.Mode != GizmoMode.Measure)
            SetAllowMeasurement(false);

        if (measureToolActive)
            HandleUserInput();
    }

    private void HandleUserInput()
    {
        var ray = ServiceLocator.GetService<CameraModeChanger>().ActiveCamera.ScreenPointToRay(Input.mousePosition);
        //var planeIntersect = RestrictionChecker.ActivePerceel.PerceelPlane.Raycast(ray, out float enter);
        var testPlane = RestrictionChecker.ActivePerceel.PerceelPlane;
        testPlane.distance *= -1; // needed because distance is defined from plane to origin instead of from origin to plane
        var planeIntersect = testPlane.Raycast(ray, out float enter);

        bool isValidMeshPoint = IsHoveringOverValidPoint(out var hoverPoint);

        if (isValidMeshPoint)
            mousePositionInWorld = hoverPoint.transform.position;
        else if (planeIntersect)
            mousePositionInWorld = ray.origin + ray.direction * enter;

        if (ObjectClickHandler.GetClickOnObject(true, LayerMask.GetMask("SelectionPoints")))
        {
            if (!isValidMeshPoint)
            {
                hoverPoint = Instantiate(cursorPointPrefab);
                hoverPoint.PointScale = 0;
                hoverPoint.transform.position = mousePositionInWorld;
            }

            // if clicking: if a valid first point is already present, and the click is on a valid second point: finalize the line. else: if clicking on a valid first point, start the line.
            if (firstPoint)
            {
                bool validStart = measureLines[measureLines.Count - 1].ValidStartPoint;
                bool validEnd = IsHoveringOverValidSecondPoint(out _);
                var line = lines[measureLines.Count - 1];

                var startPoint = validStart ? firstPoint : line.LinePoints[0];
                var endPoint = validEnd ? hoverPoint : line.LinePoints[1];

                measureLines[measureLines.Count - 1] = new MeasureLine(startPoint, endPoint, validStart, validEnd); //set the end point of the last line in the list

                line.SetDistanceLabelInteractable(validStart && validEnd);

                if (!validStart)
                    Destroy(firstPoint.gameObject);
                if (!validEnd)
                {
                    line.LinePoints[1].PointScale = 0;
                    if (!isValidMeshPoint) //destroy the instantiated second (non-mesh) point
                        Destroy(hoverPoint.gameObject);
                }
                firstPoint = null; //set that there is no longer an active first point
            }
            else
            {
                firstPoint = hoverPoint;
                if (isValidMeshPoint)
                    firstPoint.GetComponentInParent<SelectableMesh>().VisualizeActivePoint = false;
                CreateLine(isValidMeshPoint);
            }
        }
    }

    private bool IsHoveringOverValidPoint(out MeasurePoint point)
    {
        point = null;
        var activePoint = GetActivePointFromMySelectableMeshes(out var activeMesh);

        if (activePoint)
        {
            activeMesh.VisualizeActivePoint = true;
            point = activePoint;
            return true;
        }
        else
        {
            //activeMesh.VisualizeActivePoint = true;
            foreach (var mesh in otherSelectableMeshes)
            {
                if (mesh.ActivePoint)
                {
                    point = mesh.ActivePoint;
                    return true;
                }
            }
        }
        return false;
    }

    private MeasurePoint GetActivePointFromMySelectableMeshes(out SelectableMesh activeMesh)
    {
        MeasurePoint activePoint = null;
        activeMesh = null;
        foreach (var m in mySelectableMeshes)
        {
            if (m.ActivePoint)
            {
                activePoint = m.ActivePoint;
                activeMesh = m;
            }
        }

        return activePoint;
    }

    private bool IsHoveringOverValidSecondPoint(out MeasurePoint point)
    {
        point = null;
        var myActivePoint = GetActivePointFromMySelectableMeshes(out var activeMesh);
        // if first point is part of uitbouwMesh: only check other meshes.
        if (activeMesh == firstPoint.GetComponentInParent<SelectableMesh>())
        {
            foreach (var otherMesh in otherSelectableMeshes)
            {
                if (otherMesh.ActivePoint)
                {
                    point = otherMesh.ActivePoint;
                    return true;
                }
            }
        }
        // else check only uitbouwMesh
        else if (myActivePoint)
        {
            point = myActivePoint;
            return true;
        }
        return false;
    }

    private void CreateLine(bool validStart)
    {
        var line = new MeasureLine(firstPoint, null, validStart, false);
        measureLines.Add(line); //create the line, by setting the end point to null, the mouse position will be used as endpoint in DrawLines()
        var newLine = CreateNewMeasurement();
        newLine.DeleteButtonPressed += NewLine_DeleteButtonPressed;
        if (!validStart)
            newLine.LinePoints[0].PointScale = 0;
        lines.Add(newLine);
        numberOfLines++;
        newLine.EnableDeleteButton(true);
    }

    private void NewLine_DeleteButtonPressed(BuildingMeasuring source)
    {
        DeleteLine(source);
    }

    public void SetAllowMeasurement(bool allowed)
    {
        //DrawDistanceActive = allowed && !ServiceLocator.GetService<T3DInit>().HTMLData.SnapToWall;
        measureToolActive = allowed && !ServiceLocator.GetService<T3DInit>().HTMLData.SnapToWall;

        if (measureToolActive)
        {
            mySelectableMeshes = GetComponentsInChildren<SelectableMesh>();
            print(mySelectableMeshes.Length);
            foreach (var mesh in otherSelectableMeshes)
            {
                mesh.SelectVertices();
            }

            foreach (var m in mySelectableMeshes)
                m.SelectVertices();
        }
        else
        {
            foreach (var mesh in otherSelectableMeshes)
            {
                mesh.DeselectVertices();
            }
            foreach (var m in mySelectableMeshes)
                m.DeselectVertices();
        }
    }

    void DeleteLine(int index)
    {
        if (index == lines.Count - 1)
            firstPoint = null; //set that there is no longer an active first point if the unfinished last line was deleted
        Destroy(lines[index]);
        lines.RemoveAt(index);
        measureLines.RemoveAt(index);
        numberOfLines--;
    }

    void DeleteLine(MeasureLine line)
    {
        var index = measureLines.IndexOf(line);
        DeleteLine(index);
    }

    void DeleteLine(BuildingMeasuring line)
    {
        var index = lines.IndexOf(line);
        DeleteLine(index);
    }
}
