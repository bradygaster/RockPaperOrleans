namespace RockPaperOrleans.Abstractions
{
    [Serializable]
    public class Player
    {
        public string Name { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }
        public int TieCount { get; set; }
        public override bool Equals(object? obj)
            => this.Name.ToLower().Equals((obj as Player)?.Name);
    }
}