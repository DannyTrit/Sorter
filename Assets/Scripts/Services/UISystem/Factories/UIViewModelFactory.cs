using System;
using UnityEngine;

namespace Sorter.Services.UI
{
    public class UIViewModelFactory
    {
        private readonly UITypesProvider _typesProvider;

        public UIViewModelFactory(UITypesProvider typesProvider)
        {
            _typesProvider = typesProvider;
        }

        public UIViewModelBase Create(Type viewType)
        {
            var viewModelType = _typesProvider.GetViewModel(viewType);
            if (viewModelType == null)
            {
                Debug.LogError($"No matching ViewModel for view {viewType.Name}");
                return null;
            }

            return (UIViewModelBase)Activator.CreateInstance(viewModelType);
        }
    }
}