using System;
using Sorter.Services.UI;

namespace Sorter.Signals
{
    [Signal]
    public readonly struct ShowUIViewSignal
    {
        public Type ViewType { get; }
        public UIModelBase Model { get; }

        public ShowUIViewSignal(Type viewType, UIModelBase model)
        {
            ViewType = viewType;
            Model = model;
        }
    }
}