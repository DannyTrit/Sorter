using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sorter.Addressables
{
    [Serializable]
    public class PrefabAssetReference<T> : IDisposable
        where T : Component
    {
        [SerializeField] protected AssetReferenceT<GameObject> _reference;

        public GameObject Asset => _reference.Asset as GameObject;
        public string Key => _reference.RuntimeKey?.ToString() ?? string.Empty;

        public PrefabAssetReference()
        { }
        
        public PrefabAssetReference(string key)
        {
            _reference = new AssetReferenceT<GameObject>(key);
        }
        
        public async UniTask LoadAsync(CancellationToken cancellationToken)
        {
            await _reference.LoadAssetAsync().ToUniTask(cancellationToken: cancellationToken);
        }
        
        public T Instantiate(Transform parent) => GameObject.Instantiate(Asset, parent).GetComponent<T>();

        public T Instantiate() => GameObject.Instantiate(Asset).GetComponent<T>();
        
        public void Dispose()
        {
            _reference.ReleaseAsset();
        }
        
        public override string ToString() => Key;
    }
}