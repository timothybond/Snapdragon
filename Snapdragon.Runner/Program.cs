using Snapdragon;
using Snapdragon.GeneticAlgorithm;

var g = new CardAndControllerGenetics(100, c => Snapdragon.Random.Next());

const int DeckCount = 8;
const int Generations = 5;

var population = g.GetRandomPopulation(DeckCount);

var engine = new Engine(new NullLogger());

for (var i = 0; i < Generations; i++)
{
    var wins = g.RunPopulationGames(population, engine, 2);
    population = g.ReproducePopulation(population, wins);
}
