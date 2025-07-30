using Sorter.GameplayLogic.Figures;

namespace Sorter.Signals
{
    [Signal]
    public readonly struct FigureDestroyedSignal
    {
        public Figure Figure { get; } 

        public FigureDestroyedSignal(Figure figure)
        {
            Figure = figure;
        }
    }
}