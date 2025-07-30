using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sorter.Addressables;
using UnityEngine;

namespace Sorter.Services.UI
{
    public class UIViewFactory : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly Transform _root;
        private readonly UITypesProvider _typesProvider;
        private readonly UIStack _uiStack;
        private readonly UIViewModelFactory _viewModelFactory;

        public UIViewFactory(Transform root, UITypesProvider typesProvider, UIStack uiStack)
        {
            _root = root;
            _typesProvider = typesProvider;
            _uiStack = uiStack;
            _viewModelFactory = new UIViewModelFactory(typesProvider);
        }
        
        public async UniTask<UIViewBase> Create(Type viewType, UIModelBase model, CancellationToken cancellationToken)
        {
            if (_typesProvider.Contains(viewType) is false)
            {
                Debug.LogError($"Can't create view of type \"{viewType.Name}\": ui types provider does not contain such definition");
                return null;
            }
            var viewKey = _typesProvider.GetViewKey(viewType);

            var viewAsset = new PrefabAssetReference<UIViewBase>(viewKey);
            await viewAsset.AddTo(_disposables).LoadAsync(cancellationToken);
            var prefabCanvas = viewAsset.Asset.GetComponent<Canvas>();
            prefabCanvas.DisableComponent();
            var view = viewAsset.Instantiate(_root);
            
            if (view == null)
            {
                Debug.LogError($"Error on getting view component of type '{viewType.Name}'. Asset: {viewKey}");
                return null;
            }
            
            view.GetComponent<Canvas>().EnableComponent();
            view.SetDestroyDelegate(() =>
            {
                if (view == null) return;
                GameObject.Destroy(view.gameObject);
            });
            
            viewAsset.AddTo(view);
            _uiStack.Add(view);

            var viewModel = _viewModelFactory.Create(viewType); 
            
            viewModel.Initialize(model);
            viewModel.SetCloseDelegate(() =>
            {
                _uiStack.Remove(view);
                view.Close();
            });
            
            view.Initialize(viewModel);

            return view;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}