using Sorter.GameplayLogic.Figures;
using UnityEngine;

namespace Sorter.GameplayLogic
{
    [RequireComponent(typeof(Collider2D))]
    public class FigureSlot : MonoBehaviour
    {
        [SerializeField] private FigureType _figureType;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _matchAnimationStateName;

        public bool TryPut(Figure figure)
        {
            var result = figure.FigureType == _figureType;
            if (result)
            {
                _animator.Play(_matchAnimationStateName);
            }
            return result;
        }
    }
}