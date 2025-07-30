using Sorter.Addressables;
using Sorter.GameplayLogic.Figures;
using Sorter.Utils;
using UnityEngine;

namespace Sorter.Services
{
    [InstallableScriptableObject]
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObject/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [field: SerializeField] public IntRange SortFiguresAmountToWin { get; private set; }
        [field: SerializeField] public FloatRange SpawnTimeout { get; private set; }
        [field: SerializeField] public FloatRange FigureVelocity { get; private set; }
        [field: SerializeField] public int PlayerHealth { get; private set; }
        [field: SerializeField] public PrefabAssetReference<Figure> FigureAsset { get; private set; }
        [field: SerializeField] public int FiguresCacheCount { get; private set; }
        [field: SerializeField] public int EachVFXCacheCount { get; private set; }
    }
}