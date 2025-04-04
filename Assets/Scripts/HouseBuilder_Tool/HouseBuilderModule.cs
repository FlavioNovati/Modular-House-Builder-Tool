using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [RequireComponent(typeof(BoxCollider))]
    public class HouseBuilderModule : MonoBehaviour
    {
        public ModuleData ModuleData;

        public void RemoveAllComponents()
        {
            Destroy(this.GetComponent<BoxCollider>());
        }
    }
}
