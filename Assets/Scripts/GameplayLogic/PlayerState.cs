using System;
using R3;
using Sorter.Services;
using Sorter.Signals;
using UnityEngine;
using Zenject;

namespace Sorter.GameplayLogic
{
    public class PlayerState : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>();
        private readonly ReactiveProperty<int> _health = new ReactiveProperty<int>();
        
        private readonly SignalBus _signalBus;
        private readonly GameSettings _gameSettings;

        public ReadOnlyReactiveProperty<int> Score => _score;
        public ReadOnlyReactiveProperty<int> Health => _health;

        public PlayerState(GameSettings gameSettings, SignalBus signalBus)
        {
            _gameSettings = gameSettings;
            _signalBus = signalBus;
            InitSignals();
        }

        private void InitSignals()
        {
            _signalBus.GetStream<FigureDestroyedSignal>()
                .Subscribe(_ => _health.Value = Mathf.Max(0, _health.CurrentValue - 1))
                .AddTo(_disposables);
            _signalBus.GetStream<FigureSortedSignal>()
                .Subscribe(_ => _score.Value++)
                .AddTo(_disposables);
            _signalBus.GetStream<StartGameSignal>()
                .Subscribe(_ => Reset())
                .AddTo(_disposables);
        }

        private void Reset()
        {
            _score.Value = default;
            _health.Value = _gameSettings.PlayerHealth;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}