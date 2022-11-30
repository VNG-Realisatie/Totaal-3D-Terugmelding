using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Core;
using UnityEngine;

namespace T3D.Uitbouw
{
    public class SquarePolygonMeasuring : DistanceMeasurement
    {
        private SquareSurface square;

        protected override void Awake()
        {
            base.Awake();
            square = GetComponent<SquareSurface>();
        }

        protected override void Measuring_DistanceInputOverride(BuildingMeasuring source, Vector3 direction, float delta)
        {
            var deltaVector = Quaternion.Inverse(transform.rotation) * direction * delta;
            var newSize = square.Size - (Vector2)deltaVector;

            square.SetSize(newSize - new Vector2(0.0001f, 0.0001f));
        }

        protected override void DrawLines()
        {
            var corners = square.Surface.SolidSurfacePolygon.Vertices;
            var unityCorners = new Vector3[corners.Length];
            for (int i = 0; i < corners.Length; i++)
            {
                unityCorners[i] = CoordConvert.RDtoUnity(corners[i]);
            }

            DrawLine(0, unityCorners[0], unityCorners[3]); //direction matters for resize
            DrawLine(1, unityCorners[2], unityCorners[3]); //direction matters for resize
        }
    }
}
