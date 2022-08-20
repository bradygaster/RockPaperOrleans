namespace RockPaperOrleans.Abstractions
{
    [Serializable]
    public class Leaderboard
    {
        public Game ActiveGame { get; set; }
        public Turn ActiveTurn { get; set; }
    }
}
