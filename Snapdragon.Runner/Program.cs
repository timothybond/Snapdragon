using System.Reflection;
using Microsoft.Extensions.Configuration;
using Snapdragon;
using Snapdragon.Postgresql;
using Snapdragon.Runner.Experiments;

var builder = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly());
var config = builder.Build();

var connectionString = config["Repository:ConnectionString"];

var repositoryBuilder = new PostgresqlSnapdragonRepositoryBuilder(connectionString);
using (var repository = repositoryBuilder.Build())
{
    await SnapCards.All.ForEachAsync(repository.SaveCardDefinition);
}

//var helaExperiment = new HelaExperiment();
//await helaExperiment.Run(repositoryBuilder);

//var moveExperiment = new MoveDeckSoloExperiment();
//await moveExperiment.Run(repositoryBuilder);

//var kaZarExperiment = new KaZarSoloExperiment();
//await kaZarExperiment.Run(repositoryBuilder);

//var hazmatExperiment = new KaZarHazmatExperiment();
//await hazmatExperiment.Run();

var unguided = new UnguidedExperiment();
await unguided.Run(repositoryBuilder);
