using Sorter.Services.UI;
using Zenject;

namespace Sorter.UI.EndGame
{
    public class EndGameModel : UIModelBase
    {
        public int Score { get; }
        public bool IsVictory { get; }
        public SignalBus SignalBus { get; }

        public EndGameModel(int score, bool isVictory, SignalBus signalBus)
        {
            Score = score;
            IsVictory = isVictory;
            SignalBus = signalBus;
        }
    }
}