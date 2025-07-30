using Sorter.Input;
using Sorter.Services.UI;
using UnityEngine;
using Zenject;

namespace Sorter.Installers
{
    public class ServicesInstaller : MonoInstaller
    {
        [SerializeField] private UIRoot _uiRoot;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<InputController>().AsSingle().WithArguments(_uiRoot);
            Container.BindInterfacesAndSelfTo<UIService>().AsSingle().WithArguments(_uiRoot);
        }
    }
}