namespace RockPaperOrleans.Abstractions
{
    [Serializable]
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Started { get; set; } = DateTime.Now;
        public DateTime Ended { get; set; } = DateTime.MaxValue;
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Winner { get; set; }
        public int Rounds { get; set; } = 3;
        public List<Turn> Turns { get; set; } = new();
    }
}
