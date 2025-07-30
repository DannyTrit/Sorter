using Sorter.Services.UI;
using Sorter.Signals;

namespace Sorter.UI.EndGame
{
    public class EndGameViewModel : UIViewModelBase<EndGameModel>
    {
        public int Score => Model.Score;
        public bool IsVictory => Model.IsVictory;
        
        public void Restart()
        {
            Model.SignalBus.Fire<StartGameSignal>();
            Close();
        }
    }
}