namespace RockPaperOrleans.Abstractions
{
    public class SystemStatusUpdate
    {
        public DateTime DateStarted { get; set; }
        public int GrainsActive { get; set; }
        public TimeSpan TimeUp { get; set; }
        public int GamesCompleted { get; set; }
    }
}
