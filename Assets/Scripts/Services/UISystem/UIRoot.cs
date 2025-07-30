using UnityEngine;
using UnityEngine.EventSystems;

namespace Sorter.Services.UI
{
    public class UIRoot : MonoBehaviour
    {
        [field: SerializeField] public Camera Camera { get; private set; }
        [field: SerializeField] public Transform UIParent { get; private set; }
        [field: SerializeField] public EventSystem EventSystem { get; private set; }
        [field: SerializeField] public GameObject LoadingView { get; private set; }
    }
}