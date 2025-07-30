using System;
using Cysharp.Threading.Tasks;
using R3;
using Sorter.Addressables;
using Sorter.Services;
using Sorter.Signals;
using Sorter.Utils;
using UnityEngine;
using Zenject;
using URandom = UnityEngine.Random;

namespace Sorter.GameplayLogic.Figures
{
    public interface IFigureFactory
    {
        UniTask PrepareAssets();
        Figure Create(Vector3 startPosition, FigureType type);
    }
    
    public class FiguresFactory : IFigureFactory, IDisposable
    {
        private readonly PrefabAssetReference<Figure> _figureAsset;
        private readonly Transform _figuresContainer;
        private readonly FigureSpritesHolder _spritesHolder;
        private readonly GameSettings _gameSettings;
        private readonly SignalBus _signalBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly CancellationDisposable _cancellationDisposable;
        private ObjectPool<Figure> _objectPool;


        public FiguresFactory(Transform figuresContainer, FigureSpritesHolder spritesHolder, GameSettings gameSettings, SignalBus signalBus)
        {
            _figuresContainer = figuresContainer;
            _spritesHolder = spritesHolder;
            _gameSettings = gameSettings;
            _signalBus = signalBus;
            _figureAsset = gameSettings.FigureAsset;
            _cancellationDisposable = new CancellationDisposable().AddTo(_disposables);
        }

        public async UniTask PrepareAssets()
        {
            await _figureAsset.AddTo(_disposables).LoadAsync(_cancellationDisposable.Token);

            _objectPool = new ObjectPool<Figure>(
                    () => _figureAsset.Instantiate(_figuresContainer),
                    x =>
                    {
                        x.Hide();
                        x.ResetState();
                    })
                .AddTo(_disposables);
            _objectPool.Cache(_gameSettings.FiguresCacheCount);

            InitObjectPool();
        }

        private void InitObjectPool()
        {
            _signalBus.GetStream<FigureDestroyedSignal>()
                .Subscribe(x => _objectPool.TryReturn(x.Figure))
                .AddTo(_disposables);
            _signalBus.GetStream<FigureSortedSignal>()
                .Subscribe(x => _objectPool.TryReturn(x.Figure))
                .AddTo(_disposables);
            _signalBus.GetStream<StartGameSignal>()
                .Subscribe(_ => _objectPool.ReturnAll())
                .AddTo(_disposables);
            _signalBus.GetStream<EndGameSignal>()
                .Subscribe(_ => _objectPool.ForEachActive(x => x.PauseMovement()))
                .AddTo(_disposables);
        }

        public Figure Create(Vector3 startPosition, FigureType type)
        {
            var figure = _objectPool.Get();
            var sprite = _spritesHolder.GetSprite(type);
            var velocity = URandom.Range(_gameSettings.FigureVelocity.Min, _gameSettings.FigureVelocity.Max);
            figure.Initialize(startPosition, type, sprite, velocity);
            figure.Show();
            
            return figure;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}