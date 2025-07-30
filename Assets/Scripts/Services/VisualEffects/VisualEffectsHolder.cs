using Sorter.Addressables;
using UnityEngine;

namespace Sorter.Services.VFX
{
    [CreateAssetMenu(fileName = "VisualEffectsHolder", menuName = "ScriptableObject/Holder/VisualEffects")]
    public class VisualEffectsHolder : AssetsHolder
    {
        [SerializeField] private SerializableDictionary<VisualEffectType, PrefabAssetReference<VisualEffect>> _visualEffects;

        public PrefabAssetReference<VisualEffect> GetEffect(VisualEffectType type)
        {
            return GetValue(_visualEffects, type);
        }
    }
}