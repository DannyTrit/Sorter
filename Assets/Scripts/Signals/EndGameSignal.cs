namespace Sorter.Signals
{
    [Signal]
    public readonly struct EndGameSignal
    {
        public bool IsVictory { get; }
        public int Score { get; }

        public EndGameSignal(bool isVictory, int score)
        {
            IsVictory = isVictory;
            Score = score;
        }
    }
}