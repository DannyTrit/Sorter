using System.Collections.Generic;
using UnityEngine;

namespace Sorter.Services
{
    [InstallableScriptableObject]
    public abstract class AssetsHolder : ScriptableObject
    {
        protected TValue GetValue<TKey, TValue>(in IDictionary<TKey, TValue> collection, in TKey key)
        {
            if (collection.TryGetValue(key, out var value)) return value;

            Debug.LogError($"Collection \"{collection.GetType().Name}\" does not contain data with key \"{key}\"");
            return default;
        }
    }
}