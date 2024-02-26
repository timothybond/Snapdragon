using System.Reflection;
using Microsoft.Extensions.Configuration;
using Snapdragon;
using Snapdragon.Postgresql;
using Snapdragon.Runner.Experiments;

var builder = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly());
var config = builder.Build();

var connectionString = config["Repository:ConnectionString"];

var repository = new PostgresqlSnapdragonRepository(connectionString);

await SnapCards.All.ForEachAsync(repository.SaveCardDefinition);

//var moveExperiment = new MoveDeckSoloExperiment();
var kaZarExperiment = new KaZarSoloExperiment();

//moveExperiment.Run();
await kaZarExperiment.Run(repository);
