using Sorter.GameplayLogic;
using Sorter.Services.UI;

namespace Sorter.UI.Gameplay
{
    public class GameplayModel : UIModelBase
    {
        public PlayerState PlayerState { get; }

        public GameplayModel(PlayerState playerState)
        {
            PlayerState = playerState;
        }
    }
}