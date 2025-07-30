using DG.Tweening;
using R3;
using Sorter.Services.UI;
using TMPro;
using UnityEngine;

namespace Sorter.UI.Gameplay
{
    public class GameplayView : UIViewBase<GameplayViewModel>
    {
        [SerializeField] private TMP_Text _score;
        [SerializeField] private TMP_Text _health;
        [SerializeField] private Transform _healthBackground;

        private Tween _healthChangeAnimation;
        
        protected override void Initialize()
        {
            ViewModel.Score.Subscribe(x =>
            {
                _score.SetText(x.ToString());
            }).AddTo(this);
            ViewModel.Health.Subscribe(x =>
            {
                _health.SetText(x.ToString());
                if (_healthChangeAnimation == null)
                {
                    _healthChangeAnimation = _healthBackground.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f)
                        .SetEase(Ease.OutBack)
                        .SetLoops(2, LoopType.Yoyo)
                        .OnComplete(() => _healthChangeAnimation = null);
                }
            }).AddTo(this);
        }
    }
}