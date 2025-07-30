using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sorter.GameplayLogic.Figures;
using Sorter.Services;
using Sorter.Services.UI;
using Sorter.Services.VFX;
using Sorter.Signals;
using Sorter.UI.Gameplay;
using UnityEngine;
using Zenject;

namespace Sorter.GameplayLogic
{
    public class Entry : MonoBehaviour
    {
        [SerializeField] private UIRoot _uiRoot;

        private IVfxProvider _vfxProvider;
        private IFigureFactory _figureFactory;
        private GameSettings _gameSettings;
        private SignalBus _signalBus;
        private PlayerState _state;

        [Inject]
        private void Construct(IVfxProvider vfxProvider, IFigureFactory figureFactory, GameSettings gameSettings, PlayerState state, SignalBus signalBus)
        {
            _gameSettings = gameSettings;
            _vfxProvider = vfxProvider;
            _figureFactory = figureFactory;
            _state = state;
            _signalBus = signalBus;
        }

        private void  Start()
        {
            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            _uiRoot.LoadingView.Show();
            var model = new GameplayModel(_state);
            _signalBus.Fire(new ShowUIViewSignal(typeof(GameplayView), model));
            
            await _figureFactory.PrepareAssets();
            await UniTask.WhenAll(Enum.GetValues(typeof(VisualEffectType))
                .Cast<VisualEffectType>()
                .Except(new[] { VisualEffectType.Undefined })
                .Select(x => _vfxProvider.CacheVfx(x, _gameSettings.EachVFXCacheCount)));

            _signalBus.Fire<StartGameSignal>();
            _uiRoot.LoadingView.Hide();
        }
    }
}