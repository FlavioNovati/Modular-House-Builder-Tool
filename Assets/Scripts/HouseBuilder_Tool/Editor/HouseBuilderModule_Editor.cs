using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tool.ModularHouseBuilder
{
    [CustomEditor(typeof(HouseBuilderModule))]
    public class HouseBuilderModule_Editor : Editor
    {
        private HouseBuilderModule _module;
        private MeshFilter _meshFilter;

        private bool _useBoundsForCollisions = true;

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
            return;
        }

        public override void OnInspectorGUI()
        {
            _useBoundsForCollisions = GUILayout.Toggle(_useBoundsForCollisions, "Use Bounds for Collision", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Update Collider", GUILayout.ExpandWidth(true)))
                UpdateCollider();
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
            Vector3 colliderSize = _meshFilter.sharedMesh.bounds.extents * 2f;
            colliderSize.x *= meshScale.x;
            colliderSize.z *= meshScale.z;
            colliderSize.y *= meshScale.y;
            colliderSize = Quaternion.Euler(meshRotation) * colliderSize;

            //Set Collider Size and Offset
            BoxCollider boxCollider = _module.gameObject.GetComponent<BoxCollider>();
            boxCollider.size = colliderSize;
        }
    }
}