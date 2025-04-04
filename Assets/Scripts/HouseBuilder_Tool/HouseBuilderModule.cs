using UnityEngine;
using UnityEditor.SceneManagement;
using static UnityEditor.PlayerSettings;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tool.ModularHouseBuilder
{
    public class HouseBuilderModule : MonoBehaviour
    {
        public ModuleData ModuleData;

#if UNITY_EDITOR
        const float HANDLE_DISTANCE = 0.15f;

        private void OnDrawGizmos()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                return;

            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            //Draw Orign
            Handles.SphereHandleCap(-1, Vector3.zero, Quaternion.identity, 0.01f, EventType.Repaint);

            //Draw Object Extension
            Vector3 objectExtension = ModuleData.Extension;
            Vector3 offset = ModuleData.CenterOffset;
            Handles.DrawWireCube(offset, objectExtension);

            Vector3 yHandlePos = new Vector3(offset.x, objectExtension.y + HANDLE_DISTANCE, offset.z);
            Vector3 xHandlePos = new Vector3(objectExtension.x / 2f + HANDLE_DISTANCE, objectExtension.y / 2f, offset.z);
            Vector3 zHandlePos = new Vector3(offset.x, objectExtension.y / 2f, -objectExtension.z / 2f - HANDLE_DISTANCE);

            //Draw Buttons
            //Y
            DrawHandleCap(0, yHandlePos, Vector3.up, 0.5f, Color.green);
            //X
            DrawHandleCap(1, xHandlePos, Vector3.right, 0.5f, Color.red);
            //Z
            DrawHandleCap(2, zHandlePos, Vector3.forward, 0.5f, Color.blue);
            
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Debug.Log(controlID);

            switch(Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseMove:
                    break;
                
                case EventType.MouseUp:
                    break;

                case EventType.MouseDown:
                    if(controlID == HandleUtility.nearestControl && Event.current.button == 0)
                    {
                        Debug.Log(controlID);
                        GUIUtility.hotControl = controlID;
                        Event.current.Use();
                    }
                    break;
            }
        }

        private void DrawHandleCap(int controllID, Vector3 startPos, Vector3 direction, float size, Color color)
        {
            Handles.color = color;
            Handles.ArrowHandleCap(controllID, startPos, Quaternion.LookRotation(direction), size, EventType.Repaint);
            HandleUtility.AddControl(controllID, 0.25f);
        }

        /*
         * case EventType.MouseDown:
                // Log the nearest control ID if the click is near or directly above
                // the solid disc handle.
                if (controlID == HandleUtility.nearestControl && evt.button == 0)
                {
                    Debug.Log($"The nearest control is {controlID}.");

                    GUIUtility.hotControl = controlID;
                    evt.Use();
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID && evt.button == 0)
                {
                    GUIUtility.hotControl = 0;
                    evt.Use();
                }
                break
        */
#endif
    }
}
