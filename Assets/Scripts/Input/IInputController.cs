using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sorter.Input
{
    public interface IInputController
    {
        const float RAYCAST_DISTANCE = 50;
        
        Vector2 PointerPosition { get; }
        Observable<Vector2> ObservePointerUp();
        Observable<Vector2> ObservePointerDown();
        Observable<RaycastHit2D> ObservePointerClickOnObject();
        Observable<RaycastResult> ObservePointerClickOnUI();
        Observable<Vector2> ObservePointerPosition();
    }
}