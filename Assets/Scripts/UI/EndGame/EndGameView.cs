using Sorter.Services.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorter.UI.EndGame
{
    public class EndGameView : UIViewBase<EndGameViewModel>
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private TMP_Text _scoreText;
        [Space]
        [SerializeField] private GameObject[] _victoryEndGameObjects;
        [SerializeField] private GameObject[] _defeatEndGameObjects;

        protected override void Initialize()
        {
            _restartButton.ClearBind(ViewModel.Restart);
            _scoreText.SetText(ViewModel.Score.ToString());

            _victoryEndGameObjects.ForEach(x => x.SetActive(ViewModel.IsVictory));
            _defeatEndGameObjects.ForEach(x => x.SetActive(ViewModel.IsVictory is false));
        }
    }
}