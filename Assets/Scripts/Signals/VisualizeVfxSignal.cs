using Sorter.Services.VFX;
using UnityEngine;

namespace Sorter.Signals
{
    [Signal]
    public readonly struct VisualizeVfxSignal
    {
        public VisualEffectType EffectType { get; }
        public Vector3 Position { get; }

        public VisualizeVfxSignal(VisualEffectType effectType, Vector3 position)
        {
            EffectType = effectType;
            Position = position;
        }
    }
}