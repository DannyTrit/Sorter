using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sorter.GameplayLogic.Figures;
using Sorter.Input;
using Sorter.Services;
using Sorter.Services.VFX;
using Sorter.Signals;
using Sorter.UI.EndGame;
using Sorter.Utils;
using UnityEngine;
using Zenject;
using URandom = UnityEngine.Random;

namespace Sorter.GameplayLogic
{
    public class GameController : IDisposable
    {
        private readonly Camera _camera;
        private readonly GameComponents _gameComponents;
        private readonly SignalBus _signalBus;
        private readonly IInputController _inputController;
        private readonly PlayerState _state;
        private readonly IFigureFactory _figuresFactory;
        private readonly GameSettings _gameSettings;
        private CancellationDisposable _endGameCancellationDisposable;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private ObjectPool<Figure> _objectPool;

        private Figure _selectedFigure;
        private int _requiredSortFiguresAmount;
        
        private IReadOnlyList<FigureType> FigureTypes => Enum.GetValues(typeof(FigureType)).Cast<FigureType>().Except(new []{FigureType.Undefined}).ToList();

        public GameController(Camera camera, GameComponents gameComponents, IFigureFactory figuresFactory, IInputController inputController, PlayerState state, GameSettings gameSettings, SignalBus signalBus)
        {
            _camera = camera;
            _gameComponents = gameComponents;
            _signalBus = signalBus;
            _inputController = inputController;
            _state = state;
            _figuresFactory = figuresFactory;
            _gameSettings = gameSettings;

            InitInput();
            InitSignals();
        }

        private void InitInput()
        {
            _inputController.ObservePointerClickOnObject()
                .Where(x => _selectedFigure is null && x.collider.TryGetComponent<Figure>(out var figure) && figure.IsSelected is false)
                .Subscribe(x =>
                {
                    var figure = x.collider.GetComponent<Figure>();
                    _selectedFigure = figure;
                    figure.Select();
                }).AddTo(_disposables);
            
            _inputController.ObservePointerPosition()
                .Where(_ => _selectedFigure is not null)
                .Subscribe(x =>
                {
                    var position = (Vector2)_camera.ScreenToWorldPoint(x);
                    _selectedFigure.transform.position = position;
                })
                .AddTo(_disposables);

            _inputController.ObservePointerUp()
                .Where(_ => _selectedFigure is not null)
                .Subscribe(x =>
                {
                    var ray = _camera.ScreenToWorldPoint(x);
            
                    var hitInfo = Physics2D.Raycast(ray, Vector2.zero, IInputController.RAYCAST_DISTANCE,  LayerMask.GetMask("FigureSlot"));
                    
                    if (hitInfo.collider is not null && hitInfo.collider.TryGetComponent<FigureSlot>(out var slot))
                    {
                        var figurePosition = _selectedFigure.transform.position;
                        
                        if (slot.TryPut(_selectedFigure))
                        {
                            var effectType = GetMatchEffectType(_selectedFigure.FigureType);
                            _signalBus.Fire(new VisualizeVfxSignal(effectType, figurePosition));
                            _signalBus.Fire(new FigureSortedSignal(_selectedFigure));
                        }
                        else
                        {
                            _signalBus.Fire(new VisualizeVfxSignal(VisualEffectType.DestroySmoke, figurePosition));
                            _signalBus.Fire(new FigureDestroyedSignal(_selectedFigure));
                        }
                        
                        _selectedFigure = null;
                    }
                    else
                    {
                        _selectedFigure.ReturnOnPosition();
                        _selectedFigure = null;
                    }
                    
                })
                .AddTo(_disposables);
        }

        private void InitSignals()
        {
            _signalBus.GetStream<StartGameSignal>()
                .Subscribe(_ => StartGame().Forget())
                .AddTo(_disposables);
            _signalBus.GetStream<EndGameSignal>()
                .Subscribe(x => EndGame(x.Score, x.IsVictory))
                .AddTo(_disposables);
            
            _state.Health.Where(x => x <= 0)
                .Subscribe(x => _signalBus.Fire(new EndGameSignal(false, default)));
            _state.Score.Where(x => x >= _requiredSortFiguresAmount)
                .Subscribe(x => _signalBus.Fire(new EndGameSignal(true, x)));
        }

        private VisualEffectType GetMatchEffectType(FigureType type)
        {
            return type switch
            {
                FigureType.Square => VisualEffectType.MatchSquare,
                FigureType.Circle => VisualEffectType.MatchCircle,
                FigureType.Triangle => VisualEffectType.MatchTriangle,
                FigureType.Star => VisualEffectType.MatchStar,
                _ => VisualEffectType.Undefined
            };
        }

        private async UniTaskVoid StartGame()
        {
            _requiredSortFiguresAmount = URandom.Range(_gameSettings.SortFiguresAmountToWin.Min,
                _gameSettings.SortFiguresAmountToWin.Max);

            StopFigures();
            _endGameCancellationDisposable = new CancellationDisposable().AddTo(_disposables);

            _gameComponents.Show();
            await UniTask.WhenAll(_gameComponents.StartPoints.Select(x => GenerateFigure(x, _endGameCancellationDisposable.Token)));
        }

        private async UniTask GenerateFigure(Vector3 startPosition, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var figureTypeIndex = URandom.Range(0, FigureTypes.Count);
                var figureType = FigureTypes[figureTypeIndex];
                var figure = _figuresFactory.Create(startPosition, figureType);

                figure.StartMove();
                
                var timeoutDuration = URandom.Range(_gameSettings.SpawnTimeout.Min, _gameSettings.SpawnTimeout.Max);
                if (await UniTask.Delay(TimeSpan.FromSeconds(timeoutDuration), cancellationToken: cancellationToken).SuppressCancellationThrow())
                    return;
            }
        }

        private void StopFigures()
        {
            if (_endGameCancellationDisposable is not null)
            {
                _endGameCancellationDisposable.Dispose();
                _endGameCancellationDisposable = null;
            }
        }

        private void EndGame(int score, bool isVictory)
        {
            StopFigures();
            var model = new EndGameModel(score, isVictory, _signalBus);
            _signalBus.Fire(new ShowUIViewSignal(typeof(EndGameView), model));
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}