using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using Sorter.Services.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Sorter.Input
{
    public class InputController : IInputController, IDisposable
    {
        private readonly EventSystem _eventSystem;
        private readonly Camera _camera;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private InputControls _input = new InputControls();
        
        private readonly ReactiveProperty<Vector2> _pointerPosition;
        private readonly ReactiveCommand<Vector2> _pointerUpCommand;
        private readonly ReactiveCommand<Vector2> _pointerDownCommand;
        private readonly ReactiveCommand<RaycastHit2D> _pointerClickOnObjectCommand;
        private readonly ReactiveCommand<RaycastResult> _pointerClickOnUI;

        public Vector2 PointerPosition => _pointerPosition.CurrentValue;

        private InputControls.PointerActions Pointer => _input.Pointer;

        public InputController(UIRoot uiRoot)
        {
            _eventSystem = uiRoot.EventSystem;
            _camera = uiRoot.Camera;
    
            _pointerPosition = new ReactiveProperty<Vector2>().AddTo(_disposables);
            _pointerUpCommand = new ReactiveCommand<Vector2>().AddTo(_disposables);
            _pointerDownCommand = new ReactiveCommand<Vector2>().AddTo(_disposables);
            _pointerClickOnObjectCommand = new ReactiveCommand<RaycastHit2D>().AddTo(_disposables);
            _pointerClickOnUI = new ReactiveCommand<RaycastResult>().AddTo(_disposables);

            Observable.FromEvent<InputAction.CallbackContext>(
                    x => Pointer.Position.AddTo(_disposables).performed += x,
                    x => Pointer.Position.AddTo(_disposables).performed -= x)
                .Subscribe(OnPointerPosition)
                .AddTo(_disposables);
            Observable.FromEvent<InputAction.CallbackContext>(
                    x => Pointer.Up.AddTo(_disposables).performed += x,
                    x => Pointer.Up.AddTo(_disposables).performed -= x)
                .Subscribe(OnPointerUp)
                .AddTo(_disposables);
            Observable.FromEvent<InputAction.CallbackContext>(
                    x => Pointer.Down.AddTo(_disposables).performed += x,
                    x => Pointer.Down.AddTo(_disposables).performed -= x)
                .Subscribe(OnPointerDown)
                .AddTo(_disposables);

            _input.AddTo(_disposables).Enable();
        }

        public Observable<Vector2> ObservePointerUp() => _pointerUpCommand;
        public Observable<Vector2> ObservePointerDown() => _pointerDownCommand;
        public Observable<RaycastHit2D> ObservePointerClickOnObject() => _pointerClickOnObjectCommand;
        public Observable<RaycastResult> ObservePointerClickOnUI() => _pointerClickOnUI;
        public Observable<Vector2> ObservePointerPosition() => _pointerPosition;

        private void OnPointerPosition(InputAction.CallbackContext obj) => _pointerPosition.Value = obj.ReadValue<Vector2>();
        private void OnPointerUp(InputAction.CallbackContext obj) => _pointerUpCommand.Execute(PointerPosition);

        private void OnPointerDown(InputAction.CallbackContext obj)
        {
            var position = obj.ReadValue<Vector2>();
            _pointerDownCommand.Execute(position);

            if (RaycastHitAtUI(out var info))
                _pointerClickOnUI.Execute(info);
            else if (RaycastHitAtObject(out var hitInfo, position))
                _pointerClickOnObjectCommand.Execute(hitInfo);
        }

        private bool RaycastHitAtUI(out RaycastResult clickedUI)
        {
            var eventData = new PointerEventData(_eventSystem) { position = PointerPosition };
            var uiRaycastResults = new List<RaycastResult>();

            _eventSystem.RaycastAll(eventData, uiRaycastResults);
            clickedUI = uiRaycastResults.FirstOrDefault(x => x.gameObject.layer == LayerMask.NameToLayer("UI"));

            return uiRaycastResults.Count(x => x.gameObject.layer == LayerMask.NameToLayer("UI")) > 0;
        }
        
        private bool RaycastHitAtObject(out RaycastHit2D hitInfo, Vector2 position)
        {
            hitInfo = default;

            if (_camera == null) return false;

            var ray = _camera.ScreenToWorldPoint(position);
            
            hitInfo = Physics2D.Raycast(ray, Vector2.zero, IInputController.RAYCAST_DISTANCE);

            return hitInfo.collider is not null;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            if (_input is not null)
            {
                _input.Disable();
                _input = null;
            }
        }
    }
}