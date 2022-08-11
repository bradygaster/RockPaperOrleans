namespace RockPaperOrleans.Abstractions
{
    public class Game
    {
        public Guid Id { get; set; }
        public DateTime Started { get; set; } = DateTime.Now;
        public DateTime Ended { get; set; } = DateTime.MaxValue;
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player Winner { get; set; }
        public int Rounds { get; set; } = 3;
        public Dictionary<int, Turn> Turns { get; set; } = new Dictionary<int, Turn>();
    }
}
