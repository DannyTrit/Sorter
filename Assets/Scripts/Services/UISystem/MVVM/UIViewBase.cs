using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Sorter.Services.UI
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class UIViewBase : MonoBehaviour
    {
        private Action _onDestroy;
        public abstract void Initialize(UIViewModelBase viewModelBase);
        protected virtual void Initialize() { }
        public void SetDestroyDelegate(Action onDestroy) => _onDestroy = onDestroy;

        public virtual async UniTask Show(CancellationToken cancellationToken) => await UniTask.CompletedTask;


        public void Close()
        {
            _onDestroy?.Invoke();
            _onDestroy = null;
        }
    }
    
    public abstract class UIViewBase<TViewModel> : UIViewBase
        where TViewModel : UIViewModelBase
    {
        protected TViewModel ViewModel { get; private set; }

        public sealed override void Initialize(UIViewModelBase viewModelBase)
        {
            if (viewModelBase is TViewModel viewModel)
            {
                ViewModel = viewModel;
                Initialize();
            }
            else throw new InvalidCastException($"Unable to cast ViewModel of type \"{viewModelBase?.GetType()?.Name}\" to type \"{typeof(TViewModel).Name}\" this:{this}");
        }
        
        private void OnDestroy()
        {
            ViewModel?.Dispose();
        }
    }
}