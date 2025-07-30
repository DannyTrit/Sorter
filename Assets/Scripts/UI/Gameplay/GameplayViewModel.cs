using R3;
using Sorter.Services.UI;

namespace Sorter.UI.Gameplay
{
    public class GameplayViewModel : UIViewModelBase<GameplayModel>
    {
        public ReadOnlyReactiveProperty<int> Score => Model.PlayerState.Score;
        public ReadOnlyReactiveProperty<int> Health => Model.PlayerState.Health;
    }
}