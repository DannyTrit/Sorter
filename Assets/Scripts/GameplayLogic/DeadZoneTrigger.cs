using R3;
using UnityEngine;
using Zenject;
using R3.Triggers;
using Sorter.GameplayLogic.Figures;
using Sorter.Services.VFX;
using Sorter.Signals;

namespace Sorter.GameplayLogic
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeadZoneTrigger : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider;
        
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            this.OnTriggerEnter2DAsObservable()
                .Where(x => x.TryGetComponent<Figure>(out var figure) && figure.IsSelected is false)
                .Subscribe(x =>
                {
                    signalBus.Fire(new VisualizeVfxSignal(VisualEffectType.DestroySmoke, x.transform.position));
                    signalBus.Fire(new FigureDestroyedSignal(x.GetComponent<Figure>()));
                })
                .AddTo(this);
        }

        private void OnValidate()
        {
            if (_collider is null)
                _collider = GetComponent<BoxCollider2D>();
            if (_collider.isTrigger is false)
                _collider.isTrigger = true;
        }
    }
}