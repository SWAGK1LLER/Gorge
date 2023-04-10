using System.Collections.Generic;
using UnityEngine;

namespace PathCreation {
    /// Stores state data for the path creator editor

    [System.Serializable]
    public class PathCreatorData {
        public event System.Action bezierOrVertexPathModified;
        public event System.Action bezierCreated;

        [SerializeField]
        BezierPath bezierRoute;
        VertexPath vertexRoute;

        [SerializeField]
        bool vertexPathUpToDate;

        // vertex path settings
        public float vertexPathMaxAngleError = .3f;
        public float vertexPathMinVertexSpacing = 0.01f;

        // bezier display settings
        public bool showTransformTool = true;
        public bool showPathBounds;
        public bool showPerSegmentBounds;
        public bool displayAnchorPoints = true;
        public bool displayControlPoints = true;
        public float bezierHandleScale = 1;
        public bool globalDisplaySettingsFoldout;
        public bool keepConstantHandleSize;

        // vertex display settings
        public bool showNormalsInVertexMode;
        public bool showBezierPathInVertexMode;

        // Editor display states
        public bool showDisplayOptions;
        public bool showPathOptions = true;
        public bool showVertexPathDisplayOptions;
        public bool showVertexPathOptions = true;
        public bool showNormals;
        public bool showNormalsHelpInfo;
        public int tabIndex;

        public void Initialize (bool defaultIs2D) {
            if (bezierRoute == null) {
                CreateBezier (Vector3.zero, defaultIs2D);
            }
            vertexPathUpToDate = false;
            bezierRoute.OnModified -= BezierPathEdited;
            bezierRoute.OnModified += BezierPathEdited;
        }

        public void ResetBezierPath (Vector3 centre, bool defaultIs2D = false) {
            CreateBezier (centre, defaultIs2D);
        }

        void CreateBezier (Vector3 centre, bool defaultIs2D = false) {
            if (bezierRoute != null) {
                bezierRoute.OnModified -= BezierPathEdited;
            }

            var space = (defaultIs2D) ? PathSpace.xy : PathSpace.xyz;
            bezierRoute = new BezierPath (centre, false, space);

            bezierRoute.OnModified += BezierPathEdited;
            vertexPathUpToDate = false;

            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified ();
            }
            if (bezierCreated != null) {
                bezierCreated ();
            }
        }

        public BezierPath bezierPath {
            get {
                return bezierRoute;
            }
            set {
                bezierRoute.OnModified -= BezierPathEdited;
                vertexPathUpToDate = false;
                bezierRoute = value;
                bezierRoute.OnModified += BezierPathEdited;

                if (bezierOrVertexPathModified != null) {
                    bezierOrVertexPathModified ();
                }
                if (bezierCreated != null) {
                    bezierCreated ();
                }

            }
        }

        // Get the current vertex path
        public VertexPath GetVertexPath (Transform transform) {
            // create new vertex path if path was modified since this vertex path was created
            if (!vertexPathUpToDate || vertexRoute == null) {
                vertexPathUpToDate = true;
                vertexRoute = new VertexPath (bezierPath, transform, vertexPathMaxAngleError, vertexPathMinVertexSpacing);
            }
            return vertexRoute;
        }

        public void PathTransformed () {
            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified ();
            }
        }

        public void VertexPathSettingsChanged () {
            vertexPathUpToDate = false;
            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified ();
            }
        }

        public void PathModifiedByUndo () {
            vertexPathUpToDate = false;
            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified ();
            }
        }

        void BezierPathEdited () {
            vertexPathUpToDate = false;
            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified ();
            }
        }

    }
}