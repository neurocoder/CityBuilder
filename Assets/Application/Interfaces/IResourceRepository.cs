namespace CityBuilder.Application.Interfaces
{
    public interface IResourceRepository
    {
        int Gold { get; }
        bool TrySpend(int amount);
        void AddGold(int amount);
        void SetGold(int amount);
    }
}
