namespace Snapdragon.Postgresql.Data
{
    public class CardDefinition
    {
        public CardDefinition()
        {
            Name = string.Empty;
        }

        public string Name { get; set; }

        public int Power { get; set; }

        public int Cost { get; set; }

        public static explicit operator Snapdragon.CardDefinition(CardDefinition cd) =>
            new Snapdragon.CardDefinition(cd.Name, cd.Cost, cd.Power);

        public static explicit operator CardDefinition(Snapdragon.CardDefinition cd) =>
            new CardDefinition
            {
                Name = cd.Name,
                Cost = cd.Cost,
                Power = cd.Power
            };
    }
}
