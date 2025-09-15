namespace CityBuilder.Domain.Entities
{
    public sealed class BuildingLevel
    {
        public int Level { get; }
        public int Cost { get; }
        public int IncomePerTick { get; }

        public BuildingLevel(int level, int cost, int incomePerTick)
        {
            Level = level; Cost = cost; IncomePerTick = incomePerTick;
        }
    }
}