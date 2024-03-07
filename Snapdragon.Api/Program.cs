using System.Reflection;
using Snapdragon;
using Snapdragon.Postgresql;

var configBuilder = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly());
var config = configBuilder.Build();
var connectionString = config["Repository:ConnectionString"];

if (connectionString == null)
{
    throw new InvalidOperationException(
        "Could not retrieve Repository:ConnectionString from user secrets."
    );
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var repositoryBuilder = new PostgresqlSnapdragonRepositoryBuilder(connectionString);

builder.Services.AddSingleton<ISnapdragonRepositoryBuilder>(repositoryBuilder);
builder.Services.AddScoped(serviceProvider =>
{
    var builder = serviceProvider.GetRequiredService<ISnapdragonRepositoryBuilder>();
    return builder.Build();
});

// TODO: Get rid of this
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Any",
        policy =>
        {
            policy.AllowAnyOrigin();
        }
    );
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// TODO: Get rid of this
app.UseCors("Any");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
