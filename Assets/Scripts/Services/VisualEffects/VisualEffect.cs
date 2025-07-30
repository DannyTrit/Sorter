using R3;
using UnityEngine;

namespace Sorter.Services.VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class VisualEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particles;
        
        private readonly ReactiveCommand _ended = new ReactiveCommand();
        public Observable<Unit> ObserveEnd() => _ended;

        private void Awake()
        {
            var main = _particles.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            _ended.Execute(Unit.Default);
        }
    }
}