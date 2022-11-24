using System;
using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Core;
using Netherlands3D.T3DPipeline;
using UnityEngine;

namespace T3D.Uitbouw
{
    public class SquareSurface : MonoBehaviour
    {
        public Netherlands3D.T3DPipeline.CitySurface Surface;
        //public Netherlands3D.T3DPipeline.CityGeometrySemanticsObject Semantics;

        [SerializeField]
        protected Transform meshTransform;
        [SerializeField]
        protected Transform leftBound;
        public virtual Vector3 LeftBoundPosition => leftBound.position;
        [SerializeField]
        protected Transform rightBound;
        public virtual Vector3 RightBoundPosition => rightBound.position;
        [SerializeField]
        protected Transform topBound;
        public virtual Vector3 TopBoundPosition => topBound.position;
        [SerializeField]
        protected Transform bottomBound;
        public virtual Vector3 BottomBoundPosition => bottomBound.position;

        [SerializeField]
        private SurfaceSemanticType surfaceType;

        public Vector2 Size { get; private set; }

        protected virtual void Awake()
        {
            if (!meshTransform)
                meshTransform = transform;
            //assign the meshTransform before calling awake, because the meshTransform is needed to calculate the main surface polygon
            Surface = new Netherlands3D.T3DPipeline.CitySurface();
            Surface.SetSolidSurfacePolygon(new Netherlands3D.T3DPipeline.CityPolygon(GetVertices(),GetBoundaries()));

            Surface.SemanticsObject = new CityGeometrySemanticsObject(surfaceType);
        }

        public int[] GetBoundaries()
        {
            //polygonIndex = 0; since this is the only polygon for this surface
            return new int[]
            {
                    0,
                    1,
                    2,
                    3,
            };
        }

        public Vector3Double[] GetVertices()
        {
            return new Vector3Double[] {
                    CoordConvert.UnitytoRD(GetCorner(leftBound, topBound)),
                    CoordConvert.UnitytoRD(GetCorner(leftBound, bottomBound)),
                    CoordConvert.UnitytoRD(GetCorner(rightBound, bottomBound)),
                    CoordConvert.UnitytoRD(GetCorner(rightBound, topBound)),
                };
        }

        protected virtual void Start()
        {
            RecalculateScale();
        }

        public virtual void SetSize(Vector2 size)
        {
            Size = size;
            leftBound.localPosition = new Vector3(-size.x / 2, 0, 0);
            rightBound.localPosition = new Vector3(size.x / 2, 0, 0);
            topBound.localPosition = new Vector3(0, size.y / 2, 0);
            bottomBound.localPosition = new Vector3(0, -size.y / 2, 0);

            RecalculateScale();
        }

        public void RecalculateScale()
        {
            var newScale = CalculateXYScale(leftBound, rightBound, topBound, bottomBound);
            meshTransform.localScale = newScale;
            Size = newScale;
        }

        protected static Vector3 CalculateXYScale(Transform left, Transform right, Transform top, Transform bottom)
        {
            float hDist = Vector3.Distance(left.position, right.position);
            float vDist = Vector3.Distance(top.position, bottom.position);

            return new Vector3(hDist, vDist, 1);
        }

        protected Vector3 GetCorner(Transform hBound, Transform vBound)
        {
            var plane = new Plane(-meshTransform.forward, meshTransform.position);

            var projectedHPoint = plane.ClosestPointOnPlane(hBound.position);
            var projectedVPoint = plane.ClosestPointOnPlane(vBound.position);

            float hDist = Vector3.Distance(meshTransform.position, projectedHPoint);
            float vDist = Vector3.Distance(meshTransform.position, projectedVPoint);

            var hDir = (projectedHPoint - meshTransform.position).normalized;
            var vDir = (projectedVPoint - meshTransform.position).normalized;

            return meshTransform.position + hDir * hDist + vDir * vDist;
        }

        protected virtual void Update()
        {
            Surface.SolidSurfacePolygon.Vertices = GetVertices(); //needed when requesting the verts for JSONExport
        }
    }
}
