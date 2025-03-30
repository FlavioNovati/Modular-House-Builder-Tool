using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [CreateAssetMenu(fileName = "New Module", menuName = "scriptable/Tools/ModularHouseBuilder/Module")]
    public class Module_Data : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Vector3 _extentions;
        [SerializeField] private Vector3 _pivotPosition;

        public Module_Data()
        {
            _prefab = null;
            _extentions = Vector3.one;
            _pivotPosition = Vector3.zero;
        }
    }
}