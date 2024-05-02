namespace RockPaperOrleans.Abstractions;

[GenerateSerializer]
public class Player
{
    [Id(0)]
    public string? Name { get; set; }
    [Id(1)]
    public int TotalGamesPlayed { get; set; }
    [Id(2)]
    public int WinCount { get; set; }
    [Id(3)]
    public int LossCount { get; set; }
    [Id(4)]
    public int TieCount { get; set; }
    [Id(5)]
    public int PercentWon { get; set; }
    [Id(6)]
    public bool IsActive { get; set; }
    public override bool Equals(object? obj)
        => this.Name.ToLower().Equals((obj as Player)?.Name);
    public override int GetHashCode()
        => this.Name.ToLower().GetHashCode();
}