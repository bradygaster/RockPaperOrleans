namespace RockPaperOrleans.Abstractions;

[GenerateSerializer]
public class Game
{
    [Id(0)]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Id(1)]
    public DateTime Started { get; set; } = DateTime.Now;
    [Id(2)]
    public DateTime Ended { get; set; } = DateTime.MaxValue;
    [Id(3)]
    public string? Player1 { get; set; }
    [Id(4)]
    public string? Player2 { get; set; }
    [Id(5)]
    public string? Winner { get; set; }
    [Id(6)]
    public int Rounds { get; set; } = 3;
    [Id(7)]
    public List<Turn> Turns { get; set; } = new();
}
