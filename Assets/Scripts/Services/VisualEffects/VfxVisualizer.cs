using System;
using Cysharp.Threading.Tasks;
using R3;
using Sorter.Signals;
using UnityEngine;
using Zenject;

namespace Sorter.Services.VFX
{
    public class VfxVisualizer : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IVfxProvider _vfxProvider;

        public VfxVisualizer(IVfxProvider vfxProvider, SignalBus signalBus)
        {
            _vfxProvider = vfxProvider;

            signalBus.GetStream<VisualizeVfxSignal>()
                .Where(x => x.EffectType != VisualEffectType.Undefined)
                .Subscribe(x => VisualizeEffect(x.EffectType, x.Position).Forget())
                .AddTo(_disposables);
        }
        
        private async UniTaskVoid VisualizeEffect(VisualEffectType type, Vector3 position)
        {
            var effect = await _vfxProvider.GetEffect(type);
            effect.transform.position = position;
            effect.Show();
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}