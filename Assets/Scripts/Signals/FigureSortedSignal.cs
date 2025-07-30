using Sorter.GameplayLogic.Figures;

namespace Sorter.Signals
{
    [Signal]
    public readonly struct FigureSortedSignal
    {
        public Figure Figure { get; } 

        public FigureSortedSignal(Figure figure)
        {
            Figure = figure;
        }
    }
}