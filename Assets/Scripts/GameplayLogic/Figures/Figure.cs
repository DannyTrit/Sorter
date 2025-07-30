using DG.Tweening;
using UnityEngine;

namespace Sorter.GameplayLogic.Figures
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Figure : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private float _velocity;
        private Tween _movementTween;
        private Vector3 _selectedPosition;

        public FigureType FigureType { get; private set; }
        public bool IsSelected { get; private set; }

        public void Initialize(Vector3 startPosition, FigureType figureType, Sprite sprite, float velocity)
        {
            transform.position = startPosition;
            _spriteRenderer.SetSprite(sprite);
            _velocity = velocity;
            FigureType = figureType;
        }

        public void StartMove()
        {
            if (_movementTween is not null)
                _movementTween.Kill();

            _movementTween = transform.DOBlendableMoveBy(Vector3.right, 1f / _velocity)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental)
                .SetLink(gameObject);
        }

        public void Select()
        {
            if (IsSelected) return;

            IsSelected = true;
            _selectedPosition = transform.position;
            PauseMovement();
        }

        public void PauseMovement()
        {
            _movementTween.Pause();
        }

        public void ReturnOnPosition()
        {
            transform.DOMove(_selectedPosition, 0.2f)
                .SetEase(Ease.InQuint)
                .OnComplete(() =>
                {
                    _movementTween.Play();
                    IsSelected = false;
                })
                .SetLink(gameObject);
        }

        public void ResetState()
        {
            IsSelected = false;
            transform.position = Vector3.zero;
            _selectedPosition = Vector3.zero;
            if (_movementTween is not null)
                _movementTween.Kill();
        }

        private void OnDestroy()
        {
            if (_movementTween is not null)
            {
                _movementTween.Kill();
                _movementTween = null;
            }
        }

        private void OnValidate()
        {
            if (_spriteRenderer is null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}