namespace RockPaperOrleans.Abstractions
{
    public class Player
    {
        public string Name { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }

        public static Player Tie = new Tie();
    }

    public class Tie : Player
    {
        public static string TiePlayerName = "Tie";

        public Tie()
        {
            Name = TiePlayerName;
        }

        public override bool Equals(object? obj)
        {
            return this.Name.ToLower().Equals((obj as Player)?.Name);
        }
    }
}