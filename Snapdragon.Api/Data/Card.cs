namespace Snapdragon.Api.Data
{
    public class Card
    {
        public required string Name { get; set; }

        public int Cost { get; set; }

        public int Power { get; set; }

        public static explicit operator Card(CardDefinition cd) =>
            new Card
            {
                Name = cd.Name,
                Cost = cd.Cost,
                Power = cd.Power
            };
    }
}
