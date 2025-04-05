using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Runtime.InteropServices;

namespace Tool.ModularHouseBuilder
{
    [CustomEditor(typeof(HouseBuilderModule))]
    public class HouseBuilderModule_Editor : Editor
    {
        const float HANDLE_SIZE = 0.25f;
        const float HANDLE_DISTANCE = 0.15f;

        private HouseBuilderModule _module;
        private MeshFilter _meshFilter;

        private bool _useBoundsForCollisions = true;
        private int _nearestHandle;
        private Vector2 previousMousePos;

        void OnEnable()
        {
            _module = (HouseBuilderModule)target;
            _meshFilter = _module.GetComponentInChildren<MeshFilter>();

            SceneView.duringSceneGui += DuringSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public void OnSceneGUI()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                return;

            //Draw Orign
            Handles.SphereHandleCap(-1, Vector3.zero, Quaternion.identity, 0.01f, EventType.Repaint);

            //Draw Object Extension
            Vector3 objectExtension = _module.ModuleData.Extension;
            Vector3 offset = _module.ModuleData.CenterOffset;

            //Draw Handles Cap
            Vector3 yHandlePos = new Vector3(offset.x, objectExtension.y, offset.z);
            Vector3 yHandleOffset = new Vector3(0f, HANDLE_DISTANCE, 0f);

            Vector3 xHandlePos = new Vector3(objectExtension.x / 2f, objectExtension.y / 2f, offset.z);
            Vector3 xHandleOffset = new Vector3(HANDLE_DISTANCE, 0f, 0f);

            Vector3 zHandlePos = new Vector3(offset.x, objectExtension.y / 2f, objectExtension.z / 2f);
            Vector3 zHandleOffset = new Vector3(0f, 0f, HANDLE_DISTANCE);


            int hoverIndex = -1;
            if (Event.current.type == EventType.Repaint)
            {
                hoverIndex = HandleUtility.nearestControl;

                Handles.color = hoverIndex == 11 ? Color.magenta : Color.green;
                DrawHandleCap(11, yHandlePos + yHandleOffset, Vector3.up, HANDLE_SIZE, EventType.Repaint);

                Handles.color = hoverIndex == 12 ? Color.magenta : Color.red;
                DrawHandleCap(12, xHandlePos + xHandleOffset, Vector3.right, HANDLE_SIZE, EventType.Repaint);

                Handles.color = hoverIndex == 13 ? Color.magenta : Color.blue;
                DrawHandleCap(13, zHandlePos + zHandleOffset, Vector3.back, HANDLE_SIZE, EventType.Repaint);
            }

            if (Event.current.type == EventType.Layout)
            {
                DrawHandleCap(11, yHandlePos + yHandleOffset, Vector3.up, HANDLE_SIZE, EventType.Layout);
                DrawHandleCap(12, xHandlePos + xHandleOffset, Vector3.right, HANDLE_SIZE, EventType.Layout);
                DrawHandleCap(13, zHandlePos + zHandleOffset, Vector3.back, HANDLE_SIZE, EventType.Layout);
            }
            

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                _nearestHandle = HandleUtility.nearestControl;
                previousMousePos = Event.current.mousePosition;

                Undo.RegisterCompleteObjectUndo(_module.ModuleData, "Module Modified");
                Undo.FlushUndoRecordObjects();
            }

            if (Event.current.type == EventType.MouseDrag)
            {
                if(_nearestHandle == 11)
                {
                    float move = HandleUtility.CalcLineTranslation(previousMousePos, Event.current.mousePosition, yHandlePos + yHandleOffset, Vector3.up);
                    _module.ModuleData.Extension.y += move;
                    _module.ModuleData.Extension.y = Mathf.Clamp(_module.ModuleData.Extension.y, 0, float.MaxValue);
                }
                if(_nearestHandle == 12)
                {
                    float move = HandleUtility.CalcLineTranslation(previousMousePos, Event.current.mousePosition, xHandlePos + xHandleOffset, Vector3.right);
                    _module.ModuleData.Extension.x += move;
                    _module.ModuleData.Extension.x = Mathf.Clamp(_module.ModuleData.Extension.x, 0, float.MaxValue);
                }
                if(_nearestHandle == 13)
                {
                    float move = HandleUtility.CalcLineTranslation(previousMousePos, Event.current.mousePosition, zHandlePos + zHandleOffset, Vector3.forward);
                    _module.ModuleData.Extension.z += move;
                    _module.ModuleData.Extension.z = Mathf.Clamp(_module.ModuleData.Extension.z, 0, float.MaxValue);
                }

                previousMousePos = Event.current.mousePosition;
                SceneView.RepaintAll();
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                _nearestHandle = -1;
                previousMousePos = Vector2.zero;
            }
        }

        private void DrawHandleCap(int controlID, Vector3 startPos, Vector3 direction, float size, EventType eventType)
            => Handles.ArrowHandleCap(controlID, startPos, Quaternion.LookRotation(direction), size, eventType);

        public override void OnInspectorGUI()
        {
            _useBoundsForCollisions = GUILayout.Toggle(_useBoundsForCollisions, "Use Bounds for Collision", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Update Collider", GUILayout.ExpandWidth(true)))
                UpdateCollider();
            if (GUILayout.Button("Reset Extension", GUILayout.ExpandWidth(true)))
                ResetExtension();
        }

        private void DuringSceneGUI(SceneView view)
        {
            ModuleData moduleData = _module.ModuleData;
            
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if(prefabStage != null)
            {
                //Is in prefab Mode
                if (prefabStage.IsPartOfPrefabContents(_module.gameObject))
                    DrawInPrefabMode(view);
            }
            else
            {
                //Is no in prefab mode
                DrawInGameSceneMode(view);
            }

            //Draw Gizsmo
            //Draw Collision Shape
            //Add Collision?
            //Draw Collision that will be removed

            //Confirm
        }
        
        private void DrawInPrefabMode(SceneView view)
        {

        }

        private void DrawInGameSceneMode(SceneView view)
        {

        }

        private void UpdateCollider()
        {
            //Get Params
            Vector3 meshScale = _meshFilter.gameObject.transform.lossyScale;
            Vector3 meshRotation = _meshFilter.gameObject.transform.eulerAngles;
            
            //Get Collider Size
            Vector3 colliderSize = _module.ModuleData.Extension;
            colliderSize.x *= meshScale.x;
            colliderSize.z *= meshScale.z;
            colliderSize.y *= meshScale.y;
            colliderSize = Quaternion.Euler(meshRotation) * colliderSize;

            //Set Collider Size and Offset
            BoxCollider boxCollider = _module.gameObject.GetComponent<BoxCollider>();
            boxCollider.size = colliderSize;
        }

        private void ResetExtension()
        {
            //Get Params
            Vector3 meshScale = _meshFilter.gameObject.transform.lossyScale;
            Vector3 meshRotation = _meshFilter.gameObject.transform.eulerAngles;

            //Get Collider Size
            Vector3 extendsSize = _meshFilter.sharedMesh.bounds.extents * 2f;
            //TODO: Fix
        }
    }
}