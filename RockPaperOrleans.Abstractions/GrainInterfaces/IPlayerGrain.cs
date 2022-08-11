using Orleans;

namespace RockPaperOrleans.Abstractions.GrainInterfaces
{
    internal interface IPlayerGrain : IGrainWithStringKey
    {
        Task Play(Play play);
    }
}
