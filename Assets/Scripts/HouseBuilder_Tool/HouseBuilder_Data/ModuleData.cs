using UnityEditor;
using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [CreateAssetMenu(fileName = "New Module", menuName = "scriptable/Tools/ModularHouseBuilder/Module")]
    public class ModuleData : ScriptableObject
    {
        [SerializeField] public GameObject _prefab;
        [SerializeField] public Vector3 Extension;
        [SerializeField] public Vector3 CenterOffset;

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

        public Texture ModulePreview => _modulePreview;

        public ModuleData()
        {
            _prefab = null;
            Extension = Vector3.one;
            CenterOffset = Vector3.zero;
            _modulePreview = null;
        }

        public ModuleData(GameObject prefab)
        {
            _prefab = prefab;
            Extension = Vector3.one;
            CenterOffset = Vector3.zero;
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