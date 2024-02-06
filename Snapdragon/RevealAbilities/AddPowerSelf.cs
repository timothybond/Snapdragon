using Snapdragon.Calculations;
using Snapdragon.TargetFilters;

namespace Snapdragon.RevealAbilities
{
    public record AddPowerSelf(IPowerCalculation<Card> Power) : AddPower(new Self(), Power)
    {
        public AddPowerSelf(int Power)
            : this(new Constant<Card>(Power)) { }
    }
}
