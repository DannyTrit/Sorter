using System.Linq;
using System.Reflection;
using Sorter.GameplayLogic;
using Sorter.Input;
using Sorter.Services.UI;
using UnityEngine;
using Zenject;

namespace Sorter.Installers
{
    public class ServicesInstaller : MonoInstaller
    {
        [SerializeField] private UIRoot _uiRoot;
        [SerializeField] private GameComponents _gameComponents;
        
        public override void InstallBindings()
        {
            BindSignals();
            
            Container.BindInterfacesTo<InputController>().AsSingle().WithArguments(_uiRoot);
            Container.BindInterfacesAndSelfTo<UIService>().AsSingle().WithArguments(_uiRoot);
        }
        
        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsDefined(typeof(SignalAttribute), false))
                .ForEach(signal => Container.DeclareSignal(signal));
        }
    }
}