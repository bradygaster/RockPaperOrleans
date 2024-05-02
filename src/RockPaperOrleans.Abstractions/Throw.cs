namespace RockPaperOrleans.Abstractions
{
    [GenerateSerializer]
    public class Throw
    {
        [Id(0)]
        public string? Player { get; set; }
        [Id(1)]
        public Play Play { get; set; }
    }
}
