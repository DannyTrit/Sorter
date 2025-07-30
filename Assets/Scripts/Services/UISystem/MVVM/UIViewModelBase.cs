using System;

namespace Sorter.Services.UI
{
    public abstract class UIViewModelBase : IDisposable
    {
        private Action _onClose;
        public abstract void Initialize(UIModelBase baseModel);
        public void SetCloseDelegate(Action onClose) => _onClose = onClose;
        public void Close() => _onClose?.Invoke();
        public abstract void Dispose();
    }
    
    public abstract class UIViewModelBase<TModel> : UIViewModelBase
        where TModel : UIModelBase
    {
        protected TModel Model { get; private set; }

        public sealed override void Initialize(UIModelBase baseModel)
        {
            if (baseModel is TModel model)
            {
                Model = model;
                Initialize();
            }
            else throw new InvalidCastException($"Unable to cast Model of type \"{baseModel?.GetType()?.Name}\" to type \"{typeof(TModel).Name}\" this:{this}");
        }
        
        protected virtual void Initialize() { }
        public sealed override void Dispose()
        {
            DisposeInternal();
            Model.Dispose();
        }
        
        protected virtual void DisposeInternal() { }
    }
}