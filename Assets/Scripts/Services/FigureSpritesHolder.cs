using Sorter.GameplayLogic.Figures;
using UnityEngine;

namespace Sorter.Services
{
    [CreateAssetMenu(fileName = "FigureSpritesHolder", menuName = "ScriptableObject/Holder/FigureSprites")]
    public class FigureSpritesHolder : AssetsHolder
    {
        [SerializeField] private SerializableDictionary<FigureType, Sprite> _sprites;

        public Sprite GetSprite(FigureType type)
        {
            return GetValue(_sprites, type);
        }
    }
}