using UnityEngine;
using UnityEngine.EventSystems;

namespace Sorter.Services.UI
{
    public class UIRoot : MonoBehaviour
    {
        [field: SerializeField] public Transform UIParent { get; private set; }
    }
}