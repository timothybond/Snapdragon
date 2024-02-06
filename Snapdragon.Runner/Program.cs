using System.Collections.Immutable;
using System.Globalization;
using CsvHelper;
using Snapdragon;
using Snapdragon.GeneticAlgorithm;
using Snapdragon.Runner;

const int DeckCount = 32;
const int Generations = 50;

var general = new CardGenetics(
    SnapCards.All,
    new MonteCarloSearchController(5),
    100,
    c => Snapdragon.Random.Next()
);
var generalPopulation = general.GetRandomPopulation(DeckCount);

var vulturePinned = new PartiallyFixedGenetics(
    ImmutableList.Create(SnapCards.ByName["Vulture"]),
    SnapCards.All,
    new MonteCarloSearchController(5),
    100,
    c => Snapdragon.Random.Next()
);
var vulturePopulation = vulturePinned.GetRandomPopulation(DeckCount);

var engine = new Engine(new NullLogger());

var generalCardCounts = new List<List<int>>();
var vultureCardCounts = new List<List<int>>();

generalCardCounts.Add(Log.GetCardCounts(generalPopulation));
vultureCardCounts.Add(Log.GetCardCounts(vulturePopulation));

var combinedPopulations = new List<IReadOnlyList<IGeneSequence>>
{
    generalPopulation,
    vulturePopulation
};

for (var i = 0; i < Generations; i++)
{
    var wins = general.RunMixedPopulationGames(combinedPopulations, engine, 5);
    var generalPopulationWins = wins[0];
    var vulturePopulationWins = wins[1];

    generalPopulation = general.ReproducePopulation(generalPopulation, generalPopulationWins, 4);
    vulturePopulation = vulturePinned.ReproducePopulation(
        vulturePopulation,
        generalPopulationWins,
        4
    );

    Log.LogBestDeck(i, generalPopulation, generalPopulationWins);
    Log.LogBestDeck(i, vulturePopulation, vulturePopulationWins);

    generalCardCounts.Add(Log.GetCardCounts(generalPopulation));
    vultureCardCounts.Add(Log.GetCardCounts(vulturePopulation));
}

using (var writer = new StreamWriter("general-population.csv"))
{
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        foreach (var card in SnapCards.All)
        {
            csv.WriteField(card.Name);
        }

        csv.NextRecord();

        foreach (var generation in generalCardCounts)
        {
            foreach (var value in generation)
            {
                csv.WriteField(value);
            }

            csv.NextRecord();
        }
    }
}

using (var writer = new StreamWriter("vulture-population.csv"))
{
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        foreach (var card in SnapCards.All)
        {
            csv.WriteField(card.Name);
        }

        csv.NextRecord();

        foreach (var generation in vultureCardCounts)
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
