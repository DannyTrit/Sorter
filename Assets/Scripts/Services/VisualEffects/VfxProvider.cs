using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using Sorter.Utils;
using UnityEngine;

namespace Sorter.Services.VFX
{
    public interface IVfxProvider
    {
        UniTask CacheVfx(VisualEffectType type, int count);
        UniTask<VisualEffect> GetEffect(VisualEffectType type);
    }
    
    public class VfxProvider : IVfxProvider, IDisposable
    {
        private readonly Dictionary<VisualEffectType,ObjectPool<VisualEffect>> _effectsPool;
        private readonly Transform _container;
        private readonly VisualEffectsHolder _effectHolder;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly CancellationDisposable _cancellationDisposable;

        public VfxProvider(Transform container, VisualEffectsHolder effectHolder)
        {
            _container = container;
            _effectHolder = effectHolder;
            _effectsPool = new Dictionary<VisualEffectType, ObjectPool<VisualEffect>>();
            _cancellationDisposable = new CancellationDisposable().AddTo(_disposables);
        }

        public async UniTask CacheVfx(VisualEffectType type) => await CacheVfx(type, 1);
        public async UniTask CacheVfx(VisualEffectType type, int count)
        {
            var effectAsset = _effectHolder.GetEffect(type);
            if (effectAsset is null)
                return;
            await effectAsset.AddTo(_disposables).LoadAsync(_cancellationDisposable.Token);
            effectAsset.Asset.Hide();

            var objectPool = new ObjectPool<VisualEffect>(
                () =>
                {
                    var visualEffect = effectAsset.Instantiate(_container);
                    visualEffect.ObserveEnd().Subscribe(_ => _effectsPool[type].TryReturn(visualEffect))
                        .AddTo(_disposables);
                    return visualEffect;
                },
                x => x.Hide());
            objectPool.Cache(count);

            _effectsPool.Add(type, objectPool);
        }

        public async UniTask<VisualEffect> GetEffect(VisualEffectType type)
        {
            if (_effectsPool.ContainsKey(type) is false)
                await CacheVfx(type);
            
            var effect = _effectsPool[type].Get();
            return effect;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}