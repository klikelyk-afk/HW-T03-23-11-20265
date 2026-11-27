using CarsOwnerApi;
using System.Net;
using System.Text.Json;

var dbContext = new AppDbContext();

for (var attempt = 1; attempt <= 10; attempt++)
{
    try
    {
        dbContext.Database.EnsureCreated();
        break;
    }
    catch (Exception ex) when (attempt < 10)
    {
        Console.WriteLine($"DB not ready yet (attempt {attempt}/10): {ex.Message}");
        await Task.Delay(1500);
    }
}

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

var server = new HttpListener();
server.Prefixes.Add("http://+:5000/cars/");
server.Prefixes.Add("http://+:5000/owners/");
server.Start();

Console.WriteLine("Server started on http://+:5000/cars/ and http://+:5000/owners/");

while (true)
{
    var httpContext = server.GetContext();
    var path = httpContext.Request.Url?.AbsolutePath.ToLower() ?? string.Empty;
    var response = httpContext.Response;

    response.ContentType = "application/json; charset=utf-8";

    string json;
    if (path.StartsWith("/cars"))
    {
        response.StatusCode = 200;
        json = JsonSerializer.Serialize(dbContext.Cars.OrderBy(c => c.Id).ToList(), jsonOptions);
    }
    else if (path.StartsWith("/owners"))
    {
        response.StatusCode = 200;
        json = JsonSerializer.Serialize(dbContext.Owners.OrderBy(o => o.Id).ToList(), jsonOptions);
    }
    else
    {
        response.StatusCode = 404;
        json = JsonSerializer.Serialize(new { error = "Use /cars/ or /owners/" }, jsonOptions);
    }

    await using var outputStream = new StreamWriter(response.OutputStream);
    outputStream.WriteLine(json);
    outputStream.Flush();
}