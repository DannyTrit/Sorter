using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sorter.Signals;
using Zenject;

namespace Sorter.Services.UI
{
    public class UIService : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly UIStack _uiStack;
        private readonly UIViewFactory _viewFactory;
        
        public UIService(UIRoot uiRoot, SignalBus signalBus)
        {
            var typesProvider = new UITypesProvider();
            _uiStack = new UIStack();
            _viewFactory = new UIViewFactory(uiRoot.UIParent, typesProvider, _uiStack);
            var cancellationDisposable = new CancellationDisposable().AddTo(_disposables);
            signalBus.GetStream<ShowUIViewSignal>().Subscribe(x => Show(x.ViewType, x.Model,cancellationDisposable.Token).Forget()).AddTo(_disposables);
        }

        private async UniTaskVoid Show(Type viewType, UIModelBase model, CancellationToken cancellationToken)
        {
            var view = await _viewFactory.Create(viewType, model, cancellationToken);
            await view.Show(cancellationToken);
        }

        public void Dispose()
        {
            _uiStack.CloseAll();
        }
    }
}