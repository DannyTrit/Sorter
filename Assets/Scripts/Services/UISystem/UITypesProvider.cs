using System;
using System.Collections.Generic;

namespace Sorter.Services.UI
{
    public class ViewAsset
    {
        public string Key { get; }
        public Type ViewModel { get; }
        public Type Model { get; }

        public ViewAsset(string key, Type viewModel, Type model)
        {
            Key = key;
            ViewModel = viewModel;
            Model = model;
        }
    }
    
    public class UITypesProvider
    {
        private readonly Dictionary<Type, ViewAsset> _data = new Dictionary<Type, ViewAsset>();
        public IReadOnlyDictionary<Type, ViewAsset> Data => _data;

		//UITypes Auto-generated code start
		public UITypesProvider()
		{
		}
		//UITypes Auto-generated code end


		public bool Contains(Type viewType) => _data.ContainsKey(viewType);
        
        public string GetViewKey(Type viewType) => GetValue(viewType, x => x.Key);
        
        private T GetValue<T>(Type viewType, Func<ViewAsset, T> selector) => Contains(viewType) ? selector(_data[viewType]) : default;
        
        public Type GetViewModel(Type viewType) => GetValue(viewType, x => x.ViewModel);
    }
}