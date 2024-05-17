
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
    [Id(7)]
    public bool IsKicked { get; set; }

    public override bool Equals(object? obj) => obj is Player player &&
               Name == player.Name;

    public override int GetHashCode() => HashCode.Combine(Name);
}