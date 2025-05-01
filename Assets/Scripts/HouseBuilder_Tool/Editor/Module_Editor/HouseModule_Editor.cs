using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tool.ModularHouseBuilder
{
    [CustomEditor(typeof(HouseModule))]
    public class HouseModule_Editor : Editor
    {
        const float HANDLE_SIZE = 0.25f;
        const float HANDLE_SELECTED_SIZE = 0.3f;
        const float HANDLE_DISTANCE = 0.15f;

        private Color _xAxisColor = Color.red;
        private Color _yAxisColor = Color.green;
        private Color _zAxisColor = Color.blue;

        private HouseModule _module;
        private MeshFilter _meshFilter;

        private bool _useBoundsForCollisions = true;
        private int _nearestHandle;
        private Vector2 _previousMousePos;
        
        private enum ExtensionMoved
        {
            NONE,
            UP,
            FORWARD,
            RIGHT
        }
        private ExtensionMoved _movedExtension;


        void OnEnable()
        {
            _module = (HouseModule)target;
            _meshFilter = _module.GetComponentInChildren<MeshFilter>();
        }

        private void OnDisable()
        {
            
        }

        public void OnSceneGUI()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                return;

            //Draw Orign
            Handles.SphereHandleCap(-1, Vector3.zero, Quaternion.identity, 0.01f, EventType.Repaint);

            //Draw Object Extension
            Vector3 objectExtension = _module.ModuleData.Extension;
            Vector3 offset = _module.ModuleData.ColliderCenter;

            float yExtension = _module.ModuleData.Extension.y/2f + offset.y;
            float xExtension = _module.ModuleData.Extension.x/2f + offset.x;
            float zExtension = _module.ModuleData.Extension.z/2f + offset.x;

            //Draw Handles Cap
            Vector3 yHandlePos = new Vector3(offset.x, yExtension, offset.z);
            Vector3 yHandleOffset = new Vector3(0f, HANDLE_DISTANCE, 0f);

            Vector3 xHandlePos = new Vector3(xExtension, yExtension/2f, offset.z);
            Vector3 xHandleOffset = new Vector3(HANDLE_DISTANCE, 0f, 0f);

            Vector3 zHandlePos = new Vector3(offset.x, yExtension/2f, zExtension);
            Vector3 zHandleOffset = new Vector3(0f, 0f, HANDLE_DISTANCE);

            //Draw Handles Caps
            int hoverIndex = -1;
            if (Event.current.type == EventType.Repaint)
            {
                hoverIndex = HandleUtility.nearestControl;

                Handles.color = _yAxisColor;
                float yHanldeSize = hoverIndex == 11 ? HANDLE_SELECTED_SIZE : HANDLE_SIZE;
                DrawHandleCap(11, yHandlePos + yHandleOffset, Vector3.up, yHanldeSize, EventType.Repaint);
                Handles.DotHandleCap(-1, yHandlePos, Quaternion.identity, 0.005f, EventType.Repaint);

                Handles.color = _xAxisColor;
                float xHanldeSize = hoverIndex == 12 ? HANDLE_SELECTED_SIZE : HANDLE_SIZE;
                DrawHandleCap(12, xHandlePos + xHandleOffset, Vector3.right, xHanldeSize, EventType.Repaint);
                Handles.DotHandleCap(-1, xHandlePos, Quaternion.identity, 0.005f, EventType.Repaint);

                Handles.color = _zAxisColor;
                float zHanldeSize = hoverIndex == 13 ? HANDLE_SELECTED_SIZE : HANDLE_SIZE;
                DrawHandleCap(13, zHandlePos + zHandleOffset, Vector3.forward, zHanldeSize, EventType.Repaint);
                Handles.DotHandleCap(-1, zHandlePos, Quaternion.identity, 0.005f, EventType.Repaint);

                DrawExtensionLimit(_movedExtension);
                DrawSnappingPositions(_module.ModuleData.SnappingPoints);
            }

            //Draw Handle Cap with Layout Event
            if (Event.current.type == EventType.Layout)
            {
                DrawHandleCap(11, yHandlePos + yHandleOffset, Vector3.up, HANDLE_SIZE, EventType.Layout);
                DrawHandleCap(12, xHandlePos + xHandleOffset, Vector3.right, HANDLE_SIZE, EventType.Layout);
                DrawHandleCap(13, zHandlePos + zHandleOffset, Vector3.forward, HANDLE_SIZE, EventType.Layout);
            }
            
            //Mouse Down
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                _nearestHandle = HandleUtility.nearestControl;
                _previousMousePos = Event.current.mousePosition;

                Undo.RegisterCompleteObjectUndo(_module.ModuleData, "Module Modified");
            }

            //Mouse Drag -> expand selected extension
            if (Event.current.type == EventType.MouseDrag)
            {
                ExtensionMoved movedExtention = ExtensionMoved.NONE;

                if(_nearestHandle == 11)//Y Handle
                {
                    float move = HandleUtility.CalcLineTranslation(_previousMousePos, Event.current.mousePosition, yHandlePos + yHandleOffset, Vector3.up);
                    _module.ModuleData.Extension.y += move;
                    _module.ModuleData.Extension.y = Mathf.Clamp(_module.ModuleData.Extension.y, 0, float.MaxValue);

                    movedExtention = ExtensionMoved.UP;
                    EditorUtility.SetDirty(_module.ModuleData);
                }
                if(_nearestHandle == 12)//X Handle
                {
                    float move = HandleUtility.CalcLineTranslation(_previousMousePos, Event.current.mousePosition, xHandlePos + xHandleOffset, Vector3.right);
                    _module.ModuleData.Extension.x += move;
                    _module.ModuleData.Extension.x = Mathf.Clamp(_module.ModuleData.Extension.x, 0, float.MaxValue);

                    movedExtention = ExtensionMoved.RIGHT;
                    EditorUtility.SetDirty(_module.ModuleData);
                }
                if(_nearestHandle == 13)//Z Handle
                {
                    float move = HandleUtility.CalcLineTranslation(_previousMousePos, Event.current.mousePosition, zHandlePos + zHandleOffset, Vector3.forward);
                    _module.ModuleData.Extension.z += move;
                    _module.ModuleData.Extension.z = Mathf.Clamp(_module.ModuleData.Extension.z, 0, float.MaxValue);

                    movedExtention = ExtensionMoved.FORWARD;
                    EditorUtility.SetDirty(_module.ModuleData);
                }

                _movedExtension = movedExtention;
                _previousMousePos = Event.current.mousePosition;
                SceneView.RepaintAll();
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                _nearestHandle = -1;
                _previousMousePos = Vector2.zero;
                _movedExtension = ExtensionMoved.NONE;
                HandleUtility.AddDefaultControl(0);
            }
        }

        private void DrawHandleCap(int controlID, Vector3 startPos, Vector3 direction, float size, EventType eventType)
            => Handles.ArrowHandleCap(controlID, startPos, Quaternion.LookRotation(direction), size, eventType);

        private void DrawExtensionLimit(ExtensionMoved limitType)
        {
            if(limitType == ExtensionMoved.NONE) return;

            Vector3[] bounds = new Vector3[4];

            Color fadeColor = Color.white;
            Color lineColor = Color.white;
            
            float centerX = _module.ModuleData.ColliderCenter.x;
            float centerY = _module.ModuleData.ColliderCenter.y;
            float centerZ = _module.ModuleData.ColliderCenter.z;

            float halfX = _module.ModuleData.Extension.x / 2f;
            float halfY = _module.ModuleData.Extension.y / 2f;
            float halfZ = _module.ModuleData.Extension.z / 2f;

            float x = centerX + halfX;
            float y = centerY + halfY;
            float z = centerZ + halfZ;

            switch (limitType)
            {
                case ExtensionMoved.UP:

                    bounds[0] = new Vector3(-x, +y, -z);
                    bounds[1] = new Vector3(+x, +y, -z);
                    bounds[2] = new Vector3(+x, +y, +z);
                    bounds[3] = new Vector3(-x, +y, +z);

                    lineColor = _yAxisColor;
                    fadeColor = _yAxisColor;
                    break;

                case ExtensionMoved.FORWARD:

                    bounds[0] = new Vector3(-x, -halfY + centerY, +z);
                    bounds[1] = new Vector3(+x, -halfY + centerY, +z);
                    bounds[2] = new Vector3(+x, +halfY + centerY, +z);
                    bounds[3] = new Vector3(-x, +halfY + centerY, +z);

                    lineColor = _zAxisColor;
                    fadeColor = _zAxisColor;
                    break;

                case ExtensionMoved.RIGHT:

                    bounds[0] = new Vector3(+x, -halfY + centerY, -z);
                    bounds[1] = new Vector3(+x, +halfY + centerY, -z);
                    bounds[2] = new Vector3(+x, +halfY + centerY, +z);
                    bounds[3] = new Vector3(+x, -halfY + centerY, +z);

                    lineColor = _xAxisColor;
                    fadeColor = _xAxisColor;
                    break;
            }

            fadeColor.a = 0.1f;

            Handles.color = Color.white;
            Handles.DrawSolidRectangleWithOutline(bounds, fadeColor, lineColor);
        }

        private void DrawSnappingPositions(SnappingPoint[] snappingPoints)
        {
            if(snappingPoints == null)
                return;

            foreach (SnappingPoint snappingPoint in snappingPoints)
            {
                Color pointColor = snappingPoint.UseFilter ? snappingPoint.SnappingPointFilter.ToColor() : Color.white;
                Handles.color = pointColor;
                Handles.SphereHandleCap(-1, snappingPoint.LocalPoint, Quaternion.identity, 0.05f, EventType.Repaint);
            }
        }

        public override void OnInspectorGUI()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                if (GUILayout.Button("Edit Module", GUILayout.ExpandWidth(true)))
                {
                    string assetPath = _module.ModuleData.PrefabAssetPath;
                    PrefabStageUtility.OpenPrefab(assetPath);
                }
                return;
            }

            _useBoundsForCollisions = GUILayout.Toggle(_useBoundsForCollisions, "Use Bounds for Collision", GUILayout.ExpandWidth(true));
            if(_useBoundsForCollisions)
                if (GUILayout.Button("Update Collider", GUILayout.ExpandWidth(true)))
                    UpdateCollider();

            GUILayout.Space(5f);
            if (GUILayout.Button("Reset Extension", GUILayout.ExpandWidth(true)))
                ResetExtension();

            if(GUILayout.Button("Update Snapping Points", GUILayout.ExpandWidth(true)))
            {
                _module.ModuleData.CreateSnappingPoints();
                EditorUtility.SetDirty(_module.ModuleData);
                AssetDatabase.SaveAssets();
            }
        }

        private void UpdateCollider()
        {
            //Get Collider Size
            Vector3 colliderSize = _module.ModuleData.Extension;

            //Set Collider Size and Offset
            BoxCollider boxCollider = _module.gameObject.GetComponent<BoxCollider>();
            boxCollider.size = colliderSize;
            _module.ModuleData.ColliderCenter = boxCollider.center;

            EditorUtility.SetDirty(_module);
            EditorUtility.SetDirty(_module.ModuleData);
        }

        private void ResetExtension()
        {
            //Get Params
            Vector3 meshScale = _meshFilter.gameObject.transform.lossyScale;
            Vector3 meshRotation = _module.ModuleData.Rotation;

            //Get Collider Size
            Vector3 extendsSize = _meshFilter.sharedMesh.bounds.extents * 2f;

            extendsSize.x *= meshScale.x;
            extendsSize.y *= meshScale.y;
            extendsSize.z *= meshScale.z;

            extendsSize = Quaternion.Euler(meshRotation) * extendsSize;

            extendsSize.x = Mathf.Abs(extendsSize.x);
            extendsSize.y = Mathf.Abs(extendsSize.y);
            extendsSize.z = Mathf.Abs(extendsSize.z);

            _module.ModuleData.Extension = extendsSize;
            
            SceneView.RepaintAll();
            EditorUtility.SetDirty(_module.ModuleData);
        }
    }
}