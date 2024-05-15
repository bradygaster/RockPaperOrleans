namespace RockPaperOrleans.Abstractions;

[GenerateSerializer]
public class Turn
{
    [Id(0)]
    public List<Throw> Throws { get; set; } = new();
    [Id(1)]
    public string? Winner { get; set; }
}
