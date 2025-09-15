using NUnit.Framework;
using CityBuilder.Domain.Rules;
using CityBuilder.Domain.Entities;

namespace CityBuilder.Tests.DomainTests
{
    public class UpgradeRulesTests
    {
        [Test] public void DefaultLevel_Is_Level1() { var def = UpgradeRules.GetDefaultLevel(BuildingType.House); Assert.AreEqual(1, def.Level); Assert.AreEqual(1, def.IncomePerTick); }
        [Test] public void GetNextLevel_Returns_Null_If_None() { var lvl = new BuildingLevel(3,150,4); var next = UpgradeRules.GetNextLevel(BuildingType.House, lvl); Assert.IsNull(next); }
    }
}
