using System.Globalization;
using CsvHelper;
using Snapdragon;
using Snapdragon.GeneticAlgorithm;
using Snapdragon.Runner;

const int DeckCount = 64;
const int Generations = 50;

var g = new CardGenetics(
    SnapCards.All,
    new MonteCarloSearchController(5),
    100,
    c => Snapdragon.Random.Next()
);
var population = g.GetRandomPopulation(DeckCount);

var engine = new Engine(new NullLogger());

var cardCounts = new List<List<int>>();

cardCounts.Add(Log.GetCardCounts(population));

for (var i = 0; i < Generations; i++)
{
    var wins = g.RunPopulationGames(population, engine, 5);
    population = g.ReproducePopulation(population, wins, 4);

    Log.LogBestDeck(i, population, wins);

    cardCounts.Add(Log.GetCardCounts(population));
}

using (var writer = new StreamWriter("population.csv"))
{
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        foreach (var card in SnapCards.All)
        {
            csv.WriteField(card.Name);
        }

        csv.NextRecord();

        foreach (var generation in cardCounts)
        {
            foreach (var value in generation)
            {
                csv.WriteField(value);
            }

            csv.NextRecord();
        }
    }
}

Console.WriteLine("Finished.");
