using UnityEditor;
using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [CreateAssetMenu(fileName = "New Module", menuName = "scriptable/Tools/ModularHouseBuilder/Module")]
    public class ModuleData : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Vector3 _extentions;
        [SerializeField] private Vector3 _pivotPosition;
        [SerializeField] private ModuleType _moduleType;

        [SerializeField] private Texture _modulePreview;

        public GameObject Prefab
        {
            get => _prefab;
            set => _prefab = value;
        }

        public ModuleType ModuleType
        {
            get => _moduleType;
            set
            {
                _moduleType = value;
                _modulePreview = ModuleType.ToTexture();
            }
        }
        
        public Vector3 PivotPosition
        {
            get => _pivotPosition;
            set => _pivotPosition = value;
        }

        public Vector3 Extentions
        {
            get => _extentions;
            set => _extentions = value;
        }

        public Texture ModulePreview => _modulePreview;

        public ModuleData()
        {
            _prefab = null;
            _extentions = Vector3.one;
            _pivotPosition = Vector3.zero;
            _modulePreview = null;
        }

        public ModuleData(GameObject prefab)
        {
            _prefab = prefab;
            _extentions = Vector3.one;
            _pivotPosition = Vector3.zero;
            _modulePreview = PrefabUtility.GetIconForGameObject(prefab);
        }

        public void SetPreview(Texture preview)
        {
            if (preview != null)
                _modulePreview = preview;
            else
                _modulePreview = _moduleType.ToTexture();
        }
    }
}