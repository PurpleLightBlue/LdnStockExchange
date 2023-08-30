using Lamar.Microsoft.DependencyInjection;
using Serilog;
using StockAPI.Application.Services;
using StockAPI.Domain.Repositories;
using StockAPI.Infrastructure.Repositories;
using System.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json") // Assuming you have an appsettings.json file
    .Build();

// Add services to the container.
builder.Host.UseLamar((context, registry) =>
{
    // Get the connection string from appsettings.json
    var connectionString = configuration.GetConnectionString("StockExchangeConnectionString");

    // Create and register IDbConnection
    var dbConnection = new SqlConnection(connectionString);
    registry.For<IDbConnection>().Use(dbConnection);

    // Register repositories using IDbConnection
    registry.For<IStockRepository>().Use<StockRepository>().Ctor<IDbConnection>().Is(dbConnection);
    registry.For<ITradeRepository>().Use<TradeRepository>().Ctor<IDbConnection>().Is(dbConnection);
    registry.For<IBrokerRepository>().Use<BrokerRepository>().Ctor<IDbConnection>().Is(dbConnection);

    // Register services and controllers
    registry.For<IStockService>().Use<StockService>();
    registry.For<ITradeService>().Use<TradeService>();
    registry.For<IBrokerService>().Use<BrokerService>();

    registry.AddControllers();
});

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog(logger);


//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
