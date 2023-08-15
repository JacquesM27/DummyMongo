using DummyMongo.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/laptop", async (Laptop laptop) =>
{
    MongoClient mongoClient = new("mongodb+srv://mongo:mongo@cluster0.3pprbwh.mongodb.net/?retryWrites=true&w=majority");
    var laptopsCollection = mongoClient.GetDatabase("devices").GetCollection<Laptop>("laptops");
    await laptopsCollection.InsertOneAsync(laptop);
});

app.MapGet("/laptop", () =>
{
    MongoClient mongoClient = new("mongodb+srv://mongo:mongo@cluster0.3pprbwh.mongodb.net/?retryWrites=true&w=majority");
    var laptopsCollection = mongoClient.GetDatabase("devices").GetCollection<Laptop>("laptops");
    var emptyFilter = Builders<Laptop>.Filter.Empty;
    var laptops = laptopsCollection.Find(emptyFilter).ToList();
    return laptops;
});

app.MapGet("/laptop/guid", (string guid) =>
{
    MongoClient mongoClient = new("mongodb+srv://mongo:mongo@cluster0.3pprbwh.mongodb.net/?retryWrites=true&w=majority");
    var laptopsCollection = mongoClient.GetDatabase("devices").GetCollection<Laptop>("laptops");
    var filter = Builders<Laptop>.Filter.Eq("Id", guid);
    var laptop = laptopsCollection.Find(filter).First();
    return laptop;
});

app.MapPut("/laptop/input/add", (string guid, string inputName) =>
{
    MongoClient mongoClient = new("mongodb+srv://mongo:mongo@cluster0.3pprbwh.mongodb.net/?retryWrites=true&w=majority");
    var laptopsCollection = mongoClient.GetDatabase("devices").GetCollection<Laptop>("laptops");
    var filter = Builders<Laptop>.Filter.Eq("Id", guid);
    UpdateDefinition<Laptop> update = Builders<Laptop>.Update.AddToSet<string>("Inputs", inputName);
    laptopsCollection.UpdateOne(filter, update);
    var laptop = laptopsCollection.Find(filter).First();
    return laptop;
});

app.MapDelete("/laptop", (string guid) =>
{
    MongoClient mongoClient = new("mongodb+srv://mongo:mongo@cluster0.3pprbwh.mongodb.net/?retryWrites=true&w=majority");
    var laptopsCollection = mongoClient.GetDatabase("devices").GetCollection<Laptop>("laptops");
    var filter = Builders<Laptop>.Filter.Eq("Id", guid);
    laptopsCollection.DeleteOne(filter);
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
